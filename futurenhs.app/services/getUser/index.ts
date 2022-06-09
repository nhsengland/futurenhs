import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { requestMethods } from '@constants/fetch'
import { services } from '@constants/services'
import { getCsvStringFromObject } from '@helpers/util/data'
import { ServiceError } from '..'
import { FetchResponse } from '@appTypes/fetch'
import { ApiResponse, ServiceResponse } from '@appTypes/service'
import { User } from '@appTypes/user'

export type Options = {
    cookies: Record<string, string>
}

export type Dependencies = {
    setFetchOptions?: any
    fetchJSON?: any
}

export type GetUserService = (
    options: Options,
    dependencies?: Dependencies
) => Promise<ServiceResponse<User>>

export const getUser: GetUserService = async (
    { cookies = {} },
    dependencies
): Promise<ServiceResponse<User>> => {
    // const setFetchOptions =
    //     dependencies?.setFetchOptions ?? setFetchOptionsHelper
    // const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper
    // const cookieHeader: string = getCsvStringFromObject({
    //     object: cookies,
    //     seperator: '; ',
    // })

    // const apiUrl: string = '';
    // const apiResponse: FetchResponse = await fetchJSON(
    //     apiUrl,
    //     setFetchOptions({
    //         method: requestMethods.GET,
    //         headers: {
    //             Cookie: cookieHeader,
    //         },
    //     }),
    //     1000
    // )

    // const apiData: ApiResponse<any> = apiResponse.json
    // const apiMeta: any = apiResponse.meta

    // const { ok, status, statusText } = apiMeta

    // if (!ok) {
    //     throw new ServiceError(
    //         'An unexpected error occurred when attempting to get the user',
    //         {
    //             serviceId: services.GET_USER,
    //             status: status,
    //             statusText: statusText,
    //             body: apiData,
    //         }
    //     )
    // }

    // return {
    //     data: {
    //         id: apiData?.Id ?? null,
    //         text: {
    //             userName: apiData?.FullName ?? null,
    //         },
    //         image: apiData?.UserAvatar
    //             ? {
    //                   source: apiData?.UserAvatar?.Source ?? null,
    //                   altText: apiData?.UserAvatar?.AltText ?? null,
    //               }
    //             : null,
    //     },
    // }

    throw new Error('getUser is intentionally broken')

}
