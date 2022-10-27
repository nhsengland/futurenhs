import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch'
import { ServiceError } from '..'
import { getApiPaginationQueryParams } from '@helpers/routing/getApiPaginationQueryParams'
import { getClientPaginationFromApi } from '@helpers/routing/getClientPaginationFromApi'
import { FetchResponse } from '@appTypes/fetch'
import {
    Service,
    ApiPaginatedResponse,
    ServicePaginatedResponse,
} from '@appTypes/service'
import { Pagination } from '@appTypes/pagination'
import { Group } from '@appTypes/group'
import { User } from '@appTypes/user'
import jwtHeader from '@helpers/util/jwt/jwtHeader'

declare type Options = {
    user: User
    isDeleted?: boolean
    pagination?: Pagination
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const getSiteGroups: Service = async (
    { user, isDeleted, pagination }: Options,
    dependencies?: Dependencies
): Promise<ServicePaginatedResponse<Array<Group>>> => {
    const serviceResponse: ServicePaginatedResponse<Array<Group>> = {
        data: [],
    }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const id: string = user.id
    const paginationQueryParams: string = getApiPaginationQueryParams({
        pagination,
        defaults: {
            pageNumber: 1,
            pageSize: 10,
        },
    })

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/admin/groups?${paginationQueryParams}`
    const authHeader = jwtHeader(user.accessToken)
    const apiHeaders = setFetchOptions({
        method: requestMethods.GET,
        headers: authHeader,
    })
    const apiResponse: FetchResponse = await fetchJSON(
        apiUrl,
        apiHeaders,
        defaultTimeOutMillis
    )

    const apiData: ApiPaginatedResponse<any> = apiResponse.json
    const apiMeta: any = apiResponse.meta

    const { ok, status, statusText } = apiMeta

    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to get the site groups',
            {
                serviceId: services.GET_SITE_GROUPS,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    apiData.data?.forEach((datum) => {
        serviceResponse.data.push({
            text: {
                mainHeading: datum.nameText ?? null,
                strapLine: datum.strapLineText ?? null,
            } as any,
            groupId: datum.slug,
            themeId: datum.themeId,
            isDeleted: datum.isDeleted,
            totalMemberCount: datum.memberCount ?? 0,
            totalDiscussionCount: datum.discussionCount ?? 0,
            image: datum.image
                ? {
                      src: `${datum.image?.source}`,
                      height: datum.image?.height ?? null,
                      width: datum.image?.width ?? null,
                      altText: `Group image for ${datum.nameText}`,
                  }
                : null,
            owner: {
                id: datum?.owner?.id,
                fullName: datum?.owner?.name,
            },
        })
    })

    serviceResponse.pagination = getClientPaginationFromApi({
        apiPaginatedResponse: apiData,
    })

    return serviceResponse
}
