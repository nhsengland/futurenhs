import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch'
import { ServiceError } from '..'
import { ServiceResponse } from '@appTypes/service'
import { User } from '@appTypes/user'

declare type Options = {
    headers?: Record<string, string>
    body: FormData
    user: User
    targetUserId: string
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const putSiteUser = async (
    { headers, body, user, targetUserId }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<null>> => {
    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const { id } = user

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${targetUserId}/update`

    const apiHeaders = setFetchOptions({
        method: requestMethods.PUT,
        headers: headers,
        isMultiPartForm: true,
        body: body,
    })
    console.log('test')
    for (const key in headers) { console.log('key2: ' + key + ' , value2: ' + headers[key]) }
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
            'An unexpected error occurred when attempting to update the user',
            {
                serviceId: services.PUT_SITE_USER,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    return null
}
