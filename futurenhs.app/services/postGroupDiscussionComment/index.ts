import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { requestMethods, defaultTimeOutMillis } from '@constants/fetch'
import { ServiceError } from '..'
import { Service, ServiceResponse } from '@appTypes/service'
import { User } from '@appTypes/user'
import jwtHeader from '@helpers/util/jwt/jwtHeader'

declare type Options = {
    groupId: string
    discussionId: string
    user: User
    headers: any
    body: FormData
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

/**
 * Posts a new group discussion comment
 */
export const postGroupDiscussionComment: Service = async (
    { groupId, discussionId, user, headers, body }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<null>> => {
    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const { id } = user

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/groups/${groupId}/discussions/${discussionId}/comments`
    const apiHeaders = setFetchOptions({
        method: requestMethods.POST,
        headers: headers,
        body: {
            content: body.get('content'),
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
            'An unexpected error occurred when attempting to post the group discussion comment',
            {
                serviceId: services.POST_GROUP_DISCUSSION_COMMENT,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    return null
}
