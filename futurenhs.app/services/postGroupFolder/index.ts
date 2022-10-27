import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { requestMethods, defaultTimeOutMillis } from '@constants/fetch'
import { ServiceError } from '..'
import { ServiceResponse } from '@appTypes/service'
import { User } from '@appTypes/user'
import { ServerSideFormData } from '@helpers/util/form'
import jwtHeader from '@helpers/util/jwt/jwtHeader'

declare type Options = {
    groupId: string
    folderId: string
    user: User
    headers?: any
    body: FormData | ServerSideFormData
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const postGroupFolder = async (
    { groupId, folderId, user, headers, body }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<string>> => {
    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const { id } = user

    const subFolder: string = folderId ? `/${folderId}` : ''
    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/groups/${groupId}/folders${subFolder}`
    const authHeader = jwtHeader(user.accessToken)
    const apiHeaders = setFetchOptions({
        method: requestMethods.POST,
        headers: headers,
        body: {
            Name: body.get('Name'),
            Description: body.get('Description'),
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
            'An unexpected error occurred when attempting to post the group folder',
            {
                serviceId: services.POST_GROUP_FOLDER,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    return apiData
}
