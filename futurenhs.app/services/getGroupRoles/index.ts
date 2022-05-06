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

declare type Options = {
    user: User
    groupId: string
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const getGroupRoles = async (
    { user, groupId }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<any>> => {
    const serviceResponse: ServiceResponse<any> = {
        data: null,
    }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const { id } = user 

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/roles`
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
            'An unexpected error occurred when attempting to get group roles',
            {
                serviceId: services.GET_GROUP_ROLES,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    serviceResponse.headers = headers
    serviceResponse.data = apiData

    return serviceResponse
}
