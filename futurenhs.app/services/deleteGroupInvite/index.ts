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
    ApiPaginatedResponse,
    ServicePaginatedResponse,
    ServiceResponse,
} from '@appTypes/service'
import { Pagination } from '@appTypes/pagination'
import { User } from '@appTypes/user'
import { GroupMember } from '@appTypes/group'
import { Domain } from '@appTypes/domain'
import { api } from '@constants/routes'
import jwtHeader from '@helpers/util/jwt/jwtHeader'

declare type Options = {
    headers?: any
    user: User
    inviteId: string
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const deleteGroupInvite = async (
    { headers, user, inviteId }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<null>> => {
    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper
    const domainPath = api.USER_GROUP_INVITE.replace(
        '%GROUP_INVITE_ID%',
        inviteId
    )
    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}${domainPath}`
    const apiResponse: any = await fetchJSON(
        apiUrl,
        setFetchOptions({
            headers: {
                ...headers,
                ...jwtHeader(user.accessToken),
            },
            method: requestMethods.PUT,
            body: {},
        }),
        defaultTimeOutMillis
    )
    const apiData: ApiPaginatedResponse<any> = apiResponse.json
    const apiMeta: any = apiResponse.meta

    const { ok, status, statusText } = apiMeta
    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to delete domain',
            {
                serviceId: services.DELETE_GROUP_INVITE,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }
    return null
}
