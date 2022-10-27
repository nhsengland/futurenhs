import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch'
import { ServiceError } from '..'
import { FetchResponse } from '@appTypes/fetch'
import { ApiResponse, ServiceResponse } from '@appTypes/service'
import { User } from '@appTypes/user'
import { Member } from '@appTypes/member'
import jwtHeader from '@helpers/util/jwt/jwtHeader'

declare type Options = {
    user: User
    targetUserId: string
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const getSiteUserRole = async (
    { user, targetUserId }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<Member>> => {
    const serviceResponse: ServiceResponse<Member> = {
        data: null,
    }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const { id } = user

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/admin/users/${targetUserId}/roles`
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

    const apiData: ApiResponse<any> = apiResponse.json
    const apiMeta: any = apiResponse.meta

    const { ok, status, statusText, headers } = apiMeta

    if (!ok) {
        throw new ServiceError(
            "An unexpected error occurred when attempting to get the site user's role",
            {
                serviceId: services.GET_SITE_USER_ROLE,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    serviceResponse.headers = headers
    serviceResponse.data = {
        roleId: apiData.roleId ?? '',
    }

    return serviceResponse
}
