import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch'
import { ServiceError } from '..'
import { ServiceResponse } from '@appTypes/service'
import { User } from '@appTypes/user'
import jwtHeader from '@helpers/util/jwt/jwtHeader'

declare type Options = {
    headers?: any
    body: FormData
    user: User
    targetUserId: string
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const putSiteUserRole = async (
    { headers, body, user, targetUserId }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<null>> => {
    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const { id } = user

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/admin/users/${targetUserId}/roles`

    const apiHeaders = setFetchOptions({
        method: requestMethods.PUT,
        headers: headers,
        body: {
            newRoleId: body.get('newRoleId'),
            currentRoleId: body.get('currentRoleId'),
        },
    })

    const apiResponse: any = await fetchJSON(
        apiUrl,
        apiHeaders,
        defaultTimeOutMillis
    )

    const apiMeta: any = apiResponse.meta
    const apiData: any = apiResponse.json

    const { ok, status, statusText } = apiMeta

    if (!ok) {
        throw new ServiceError(
            "An unexpected error occurred when attempting to update the user's role",
            {
                serviceId: services.PUT_SITE_USER_ROLE,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    return null
}
