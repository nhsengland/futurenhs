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
import { api } from '@constants/routes'

declare type Options = {
    user: User
    headers?: any
    body: FormData | ServerSideFormData
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const postGroupUserInvite = async (
    { user, headers = {}, body }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<null>> => {
    debugger
    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const { id } = user
    const emailAddress: FormDataEntryValue = body.get('Email')

    const gateway = process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL
    const registrationPath = api.GROUP_ADMIN_INVITE.replace('%ID%', id)
    const apiUrl: string = gateway + registrationPath
    const apiResponse: any = await fetchJSON(
        apiUrl,
        setFetchOptions({
            method: requestMethods.POST,
            headers: headers,
            body: {
                emailAddress: emailAddress,
            },
        }),
        defaultTimeOutMillis
    )

    const apiMeta: any = apiResponse.meta
    const apiData: any = apiResponse.json

    const { ok, status, statusText } = apiMeta
    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to invite a user',
            {
                serviceId: services.POST_GROUP_USER_INVITE,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    return null
}
