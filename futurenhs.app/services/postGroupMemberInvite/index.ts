import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { requestMethods, defaultTimeOutMillis } from '@constants/fetch'
import { ServiceError } from '..'
import { ServerSideFormData } from '@helpers/util/form'
import { ServiceResponse } from '@appTypes/service'
import { User } from '@appTypes/user'

declare type Options = {
    user: User
    groupId: string
    headers?: any
    body: FormData | ServerSideFormData
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const postGroupMemberInvite = async (
    { user, headers = {}, body, groupId }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<null>> => {
    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const { id } = user
    const emailAddress: FormDataEntryValue = body.get('Email')
    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/registration/invite`
    const apiBody = {
        emailAddress,
        groupSlug: groupId,
    }
    const apiResponse: any = await fetchJSON(
        apiUrl,
        setFetchOptions({
            method: requestMethods.POST,
            headers: headers,
            body: apiBody,
        }),
        defaultTimeOutMillis
    )

    const apiMeta: any = apiResponse.meta
    const apiData: any = apiResponse.json

    const { ok, status, statusText } = apiMeta

    if (!ok) {
        console.log({ ok, status, statusText })
        throw new ServiceError(
            'An unexpected error occurred when attempting to invite a group member',
            {
                serviceId: services.POST_GROUP_MEMBER_INVITE,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    } else {
        console.log('request did something')
    }

    return null
}
