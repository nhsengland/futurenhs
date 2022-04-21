import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { requestMethods, defaultTimeOutMillis } from '@constants/fetch'
import { ServiceError } from '..'
import { FetchResponse } from '@appTypes/fetch'
import { ApiResponse, ServiceResponse } from '@appTypes/service'
import { User } from '@appTypes/user'

declare type Options = {
    user: User
    groupId: string
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export type GetGroupContentSiteMapService = (
    options: Options,
    dependencies?: Dependencies
) => Promise<ServiceResponse<any>>

export const getGroupContentSiteMap = async (
    { user, groupId }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<any>> => {
    const serviceResponse: ServiceResponse<any> = {
        data: null,
    }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const id: string = user.id

    const apiUrl: string = `${
        process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL
    }/v1/groups/${groupId}/site`

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
            'An unexpected error occurred when attempting to get the group content site map',
            {
                serviceId: services.GET_GROUpP_CONTENT_SITE_MAP,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    console.log(apiData, apiMeta);

    serviceResponse.headers = headers
    serviceResponse.data = {

    }

    return serviceResponse
}
