import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch'
import { ServiceError } from '..'
import { getApiPaginationQueryParams } from '@helpers/routing/getApiPaginationQueryParams'
import { getClientPaginationFromApi } from '@helpers/routing/getClientPaginationFromApi'
import {
    ApiPaginatedResponse,
    ServicePaginatedResponse,
    ServiceResponse,
} from '@appTypes/service'
import { Pagination } from '@appTypes/pagination'
import { User } from '@appTypes/user'
import { Domain } from '@appTypes/domain'
import jwtHeader from '@helpers/util/jwt/jwtHeader'
import { api } from '@constants/routes'

declare type Options = {
    user: User
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export type FeatureFlag = {
    id: string
    name: string
    enabled: boolean
}

export const getFeatureFlags = async (
    { user }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<Array<FeatureFlag>>> => {
    const serviceResponse: ServiceResponse<Array<FeatureFlag>> = {
        data: [],
    }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}${api.ADMIN_FEATURE_FLAGS}`
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

    const { ok, status, statusText } = apiMeta
    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to get the feature flags',
            {
                serviceId: services.GET_FEATURE_FLAGS,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    serviceResponse.data = apiData

    return serviceResponse
}
