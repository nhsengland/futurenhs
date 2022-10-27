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
    groupId: string
    memberId: string
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const putGroupMemberRole = async (
    { headers, body, user, groupId, memberId }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<null>> => {
    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const { id } = user

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/groups/${groupId}/members/${memberId}/roles/update`
    const authHeader = jwtHeader(user.accessToken)
    const apiHeaders = setFetchOptions({
        method: requestMethods.PUT,
        headers: headers,
        body: {
            groupUserRoleId: body.get('groupUserRoleId'),
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
            "An unexpected error occurred when attempting to update the group member's role",
            {
                serviceId: services.PUT_GROUP_MEMBER_ROLE,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    return null
}
