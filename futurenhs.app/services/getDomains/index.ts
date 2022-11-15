import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch'
import { ServiceError } from '..'
import { getApiPaginationQueryParams } from '@helpers/routing/getApiPaginationQueryParams'
import { getClientPaginationFromApi } from '@helpers/routing/getClientPaginationFromApi'
import {
    ApiPaginatedResponse,
    ServicePaginatedResponse,
} from '@appTypes/service'
import { Pagination } from '@appTypes/pagination'
import { User } from '@appTypes/user'
import { Domain } from '@appTypes/domain'
import jwtHeader from '@helpers/util/jwt/jwtHeader'

declare type Options = {
    user: User
    pagination?: Pagination
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const getDomains = async (
    { user, pagination }: Options,
    dependencies?: Dependencies
): Promise<ServicePaginatedResponse<Array<Domain>>> => {
    const serviceResponse: ServicePaginatedResponse<Array<Domain>> = {
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
    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/registration/domains?${paginationQueryParams}`
    const apiResponse: any = await fetchJSON(
        apiUrl,
        setFetchOptions({
            method: requestMethods.GET,
            headers: {
                ...jwtHeader(user.accessToken),
            },
        }),
        defaultTimeOutMillis
    )
    const apiData: ApiPaginatedResponse<any> = apiResponse.json
    const apiMeta: any = apiResponse.meta

    const { ok, status, statusText } = apiMeta
    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to get the approved domains',
            {
                serviceId: services.GET_DOMAINS,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    apiData.data?.forEach((datum) => {
        serviceResponse.data.push({
            id: datum.id,
            domain: datum.emailDomain,
            rowVersion: datum.rowVersion,
        })
    })

    serviceResponse.pagination = getClientPaginationFromApi({
        apiPaginatedResponse: apiData,
    })

    return serviceResponse
}
