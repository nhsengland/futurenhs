import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch'
import { ServiceError } from '..'
import { ServiceResponse } from '@appTypes/service'
import { api } from '@constants/routes'

declare type Options = {
    headers?: any
    body: FormData
    subjectId: string
    emailAddress: string
    issuer: string
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const postRegisterSiteUser = async (
    { headers, body, subjectId, emailAddress, issuer }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<null>> => {
    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper
    const registerPath = api.SITE_USER_REGISTER
    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}${registerPath}`
    const apiResponse: any = await fetchJSON(
        apiUrl,
        setFetchOptions({
            method: requestMethods.POST,
            headers: headers,
            body: {
                Subject: subjectId,
                Email: emailAddress,
                Issuer: issuer,
                FirstName: body.get('firstName'),
                LastName: body.get('lastName'),
                Agreed: Boolean(body.get('terms')),
            },
        }),
        defaultTimeOutMillis
    )

    const apiMeta: any = apiResponse.meta
    const apiData: any = apiResponse.json

    const { ok, status, statusText } = apiMeta
    const { error } = apiData
    const userDomainErrorKey = 'domain'
    const hasDomainError =
        error && error.toLowerCase().includes(userDomainErrorKey)
    if (!ok) {
        if (hasDomainError) {
            throw new ServiceError(
                'Sorry, this email domain is currently not accepted on this platform',
                {
                    serviceId: services.PUT_SITE_USER,
                    status: status,
                    statusText: statusText,
                    body: apiData,
                }
            )
        }
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
