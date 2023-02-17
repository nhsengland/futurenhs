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
import jwtHeader from '@helpers/util/jwt/jwtHeader'

declare type Options = {
    user: User
    groupId: string
    headers?: any
    email: string
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const postGroupUserInvite = async (
    { user, groupId, headers = {}, email }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<null>> => {
    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const gateway = process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL
    const registrationPath = api.POST_GROUP_INVITE.replace(
        '%GROUP_ID%',
        groupId
    )
    const apiUrl: string = gateway + registrationPath
    const apiHeaders = setFetchOptions({
        method: requestMethods.POST,
        headers: headers,
        body: {
            emailAddress: email,
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
    const { error } = apiData
    const duplicateErrorKey = 'duplicate'
    const hasDuplicateError =
        error && error.toLowerCase().includes(duplicateErrorKey)
    if (!ok) {
        if (hasDuplicateError) {
            throw new ServiceError('This user is already a member', {
                serviceId: services.PUT_SITE_USER,
                status: status,
                statusText: statusText,
                body: apiData,
            })
        }
        throw new ServiceError(
            'An unexpected error occurred when attempting to invite a user',
            {
                serviceId: services.POST_GROUP_INVITE,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    return null
}
