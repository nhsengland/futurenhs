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
import { GroupMember } from '@appTypes/group'
import { mapToProfileImageObject } from '@helpers/util/data'
import jwtHeader from '@helpers/util/jwt/jwtHeader'

declare type Options = {
    user: User
    groupId: string
    memberId: string
    isForUpdate?: boolean
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const getGroupMember = async (
    { user, groupId, memberId, isForUpdate }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<GroupMember>> => {
    const serviceResponse: ServiceResponse<GroupMember> = {
        data: null,
    }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const id: string = user.id

    const apiUrl: string = `${
        process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL
    }/v1/groups/${groupId}/members/${memberId}${
        isForUpdate ? '/update' : ''
    }`
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
            'An unexpected error occurred when attempting to get the group member',
            {
                serviceId: services.GET_GROUP_MEMBER,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    serviceResponse.headers = headers
    serviceResponse.data = {
        id: apiData.id ?? '',
        firstName: apiData.firstName ?? '',
        lastName: apiData.lastName ?? '',
        email: apiData.email ?? '',
        pronouns: apiData.pronouns ?? '',
        role: apiData.role ?? '',
        joinDate: apiData.dateJoinedUtc ?? '',
        lastLogInDate: apiData.lastLoginUtc ?? '',
        image: mapToProfileImageObject(apiData.image, 'Profile image')
    }

    return serviceResponse
}
