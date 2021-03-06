import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { requestMethods, defaultTimeOutMillis } from '@constants/fetch'
import { services } from '@constants/services'
import { ServiceError } from '..'
import { ServiceResponse } from '@appTypes/service'
import { User } from '@appTypes/user'
import { ServerSideFormData } from '@helpers/util/form'

declare type Options = {
    groupId: string
    user: User
    headers?: Headers
    body: FormData | ServerSideFormData
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const postGroupDiscussion = async (
    { groupId, user, headers, body }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<null>> => {
    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const { id } = user

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/discussions`
    const apiResponse: any = await fetchJSON(
        apiUrl,
        setFetchOptions({
            method: requestMethods.POST,
            headers: headers,
            body: {
                Title: body.get('Title'),
                Content: body.get('Content'),
            },
        }),
        defaultTimeOutMillis
    )

    const apiMeta: any = apiResponse.meta
    const apiData: any = apiResponse.json

    const { ok, status, statusText } = apiMeta

    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to post the group discussion',
            {
                serviceId: services.POST_GROUP_DISCUSSION,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    return null
}
