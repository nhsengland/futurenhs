import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch'
import { ServiceError } from '..'
import { FetchResponse } from '@appTypes/fetch'
import { ApiResponse, ServiceResponse } from '@appTypes/service'
import { User } from '@appTypes/user'
import { GroupMember } from '@appTypes/group'

declare type Response = {
    group: string
}

declare type Options = {
    id: string
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const getGroupsByInvite = async (
    { id }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<Response>> => {
    const serviceResponse: ServiceResponse<Response> = {
        data: null,
    }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/users/${id}`
    const apiResponse: FetchResponse = await fetchJSON(
        apiUrl,
        setFetchOptions({ method: requestMethods.GET }),
        defaultTimeOutMillis
    )

    const apiData: ApiResponse<Response> = apiResponse.json
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
        group: apiData.group ?? '',
    }

    return serviceResponse
}
