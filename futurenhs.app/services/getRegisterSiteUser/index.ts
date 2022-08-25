import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch'
import { ServiceError } from '..'
import { FetchResponse } from '@appTypes/fetch'
import { ApiResponse, ServiceResponse } from '@appTypes/service'
import { Member } from '@appTypes/member'

declare type Options = {
    email: string
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const getRegisterSiteUser = async (
    { email }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<Member>> => {
    const serviceResponse: ServiceResponse<Member> = {
        data: null,
    }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/register/${email}`
    const apiResponse: FetchResponse = await fetchJSON(
        apiUrl,
        setFetchOptions({ method: requestMethods.GET }),
        defaultTimeOutMillis
    )

    const apiData: ApiResponse<any> = apiResponse.json
    const apiMeta: any = apiResponse.meta

    const { ok, status, statusText, headers } = apiMeta

    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to get the site user',
            {
                serviceId: services.GET_SITE_USER,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    serviceResponse.headers = headers
    serviceResponse.data = {
        id: apiData.id ?? '',
        firstName: apiData.firstName ?? '',
        lastName: apiData.lastName ?? '',
        email: apiData.email ?? '',
        pronouns: apiData.pronouns ?? '',
        image: apiData.image
            ? {
                  src: `${apiData.image?.source}`,
                  height: apiData?.image?.height,
                  width: apiData?.image?.width,
                  altText: 'Profile image',
              }
            : null,
        imageId: apiData.imageId ?? '',
        subject: apiData.subject,
    }

    return serviceResponse
}
