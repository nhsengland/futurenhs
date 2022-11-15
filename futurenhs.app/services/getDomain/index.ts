import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch'
import { ServiceError } from '..'
import { ServiceResponse } from '@appTypes/service'
import { User } from '@appTypes/user'
import { Domain } from '@appTypes/domain'
import jwtHeader from '@helpers/util/jwt/jwtHeader'

declare type Options = {
    user: User
    domainId: string
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const getDomain = async (
    { user, domainId }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<Domain>> => {
    const serviceResponse: ServiceResponse<Domain> = {
        data: null,
    }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper
    const id: string = user.id
    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/registration/domains/${domainId}`
    const apiResponse: any = await fetchJSON(
        apiUrl,
        setFetchOptions({
            method: requestMethods.GET,
            headers: {
                ...jwtHeader(user.accessToken),
            },
        }),
        defaultTimeOutMillis
    )
    const apiData: any = apiResponse.json
    const apiMeta: any = apiResponse.meta

    const { ok, status, statusText, headers } = apiMeta
    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to get the domain',
            {
                serviceId: services.GET_DOMAINS,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }
    serviceResponse.data = {
        id: apiData.id,
        domain: apiData.emailDomain,
        rowVersion: apiData.rowVersion,
    }
    serviceResponse.headers = headers
    return serviceResponse
}
