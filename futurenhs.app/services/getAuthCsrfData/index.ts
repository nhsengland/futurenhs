import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { requestMethods, defaultTimeOutMillis } from '@constants/fetch'
import { ServiceError } from '..'
import { FetchResponse } from '@appTypes/fetch'
import { ApiResponse, ServiceResponse } from '@appTypes/service'
import { Group } from '@appTypes/group'

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export type GetAuthCsrfData = (
    query: Record<string, any>,
    dependencies?: Dependencies
) => Promise<ServiceResponse<string>>

export const getAuthCsrfData = async (
    query: Record<string, any>,
    dependencies?: Dependencies
): Promise<ServiceResponse<string>> => {
    const serviceResponse: ServiceResponse<string> = {
        data: null,
    }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const baseUrl: string = process.env.NEXTAUTH_URL;
    const callBackUrlParamName: string = 'callbackUrl';
    const isCallbackUrlPresent: boolean = typeof query?.[callBackUrlParamName] === 'string';
    const isCallbackUrlValid: boolean = isCallbackUrlPresent && (query?.[callBackUrlParamName] as string).startsWith(baseUrl);
    const host: string = isCallbackUrlValid ? (query?.[callBackUrlParamName] as string) : baseUrl;
    const redirectURL = encodeURIComponent(host);

    const apiUrl: string = `${baseUrl}/api/auth/csrf?${callBackUrlParamName}=${redirectURL}`;
    const apiResponse: FetchResponse = await fetchJSON(
        apiUrl,
        setFetchOptions({ method: requestMethods.GET }),
        defaultTimeOutMillis
    )

    const apiData: ApiResponse<any> = apiResponse.json
    const apiMeta: any = apiResponse.meta

    const { headers, ok, status, statusText } = apiMeta

    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to get the auth csrf data',
            {
                serviceId: services.GET_AUTH_CSRF_DATA,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    serviceResponse.headers = headers
    serviceResponse.data = apiData.csrfToken

    return serviceResponse
}
