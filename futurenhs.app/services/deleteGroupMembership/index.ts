import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch'
import { ServiceError } from '..'
import { FetchResponse } from '@appTypes/fetch'
import { ApiResponse, ServiceResponse } from '@appTypes/service'
import { Group } from '@appTypes/group'
import { User } from '@appTypes/user'
import jwtHeader from '@helpers/util/jwt/jwtHeader'

declare type Options = {
    headers: any
    groupId: string
    user: User
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export type DeleteGroupMembershipService = (
    options: Options,
    dependencies?: Dependencies
) => Promise<ServiceResponse<Group>>

export const deleteGroupMembership = async (
    { headers, groupId, user }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<Group>> => {
    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const { id } = user

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/groups/${groupId}/members/leave`
    const authHeader = jwtHeader(user.accessToken)
    const apiHeaders = setFetchOptions({
        method: requestMethods.DELETE,
        headers: headers,
    })
    const apiResponse: FetchResponse = await fetchJSON(
        apiUrl,
        apiHeaders,
        defaultTimeOutMillis
    )

    const apiData: ApiResponse<any> = apiResponse.json
    const apiMeta: any = apiResponse.meta

    const { ok, status, statusText } = apiMeta

    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to remove the member',
            {
                serviceId: services.DELETE_GROUP_MEMBERSHIP,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    return {}
}
