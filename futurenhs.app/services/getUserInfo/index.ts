import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { requestMethods } from '@constants/fetch'
import { services } from '@constants/services'
import { ServiceError } from '..'
import { FetchResponse } from '@appTypes/fetch'
import { ApiResponse, ServiceResponse } from '@appTypes/service'
import { User } from '@appTypes/user'

export type Options = {
    subjectId: string;
    emailAddress: string;
}

export type Dependencies = {
    setFetchOptions?: any
    fetchJSON?: any
}

export type GetUserInfoService = (
    options: Options,
    dependencies?: Dependencies
) => Promise<ServiceResponse<User>>

export const getUserInfo: GetUserInfoService = async (
    { subjectId, emailAddress },
    dependencies
): Promise<ServiceResponse<User>> => {
    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const apiUrl: string = `${
        process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL
    }/v1/users/info`;
    const apiResponse: FetchResponse = await fetchJSON(
        apiUrl,
        setFetchOptions({
            method: requestMethods.POST,
            body: { subjectId, emailAddress }
        }),
        1000
    )

    // const apiData: ApiResponse<any> = apiResponse.json
    // const apiMeta: any = apiResponse.meta

    // const { ok, status, statusText } = apiMeta

    console.log(apiData, 10000);

    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to get the user info',
            {
                serviceId: services.GET_USER_INFO,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    return {
        data: {
            id: apiData?.membershipUserId ?? null,
            status: apiData.status,
            text: {
                userName: `${apiData?.firstName} ${apiData?.lastName}`,
            },
            image: apiData?.UserAvatar
                ? {
                      source: apiData?.UserAvatar?.Source ?? null,
                      altText: apiData?.UserAvatar?.AltText ?? null,
                  }
                : null,
        },
    }

}
