import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch'
import { ServiceError } from '..'
import { ServiceResponse } from '@appTypes/service'
import { api } from '@constants/routes'
import { User } from '@appTypes/user'
import jwtHeader from '@helpers/util/jwt/jwtHeader'

declare type Options = {
    headers?: any
    domain: string
    user: User
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const postDomain = async (
    { headers, domain, user }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<null>> => {
    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper
    const domainPath = api.ALLOW_DOMAIN.replace('%USER_ID%', user.id)
    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}${domainPath}`
    const apiResponse: any = await fetchJSON(
        apiUrl,
        setFetchOptions({
            method: requestMethods.POST,
            headers: {
                ...headers,
                ...jwtHeader(user.accessToken),
            },
            body: {
                EmailDomain: domain,
            },
        }),
        defaultTimeOutMillis
    )
    const apiMeta: any = apiResponse.meta
    const apiData: any = apiResponse.json

    const { ok, status, statusText } = apiMeta
    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to add the domain',
            {
                serviceId: services.ADD_DOMAIN,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    return null
}
