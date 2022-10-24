import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch'
import { ServiceError } from '..'
import { getApiPaginationQueryParams } from '@helpers/routing/getApiPaginationQueryParams'
import { getClientPaginationFromApi } from '@helpers/routing/getClientPaginationFromApi'
import { FetchResponse } from '@appTypes/fetch'
import {
    ApiPaginatedResponse,
    ServicePaginatedResponse,
    ServiceResponse,
} from '@appTypes/service'
import { Pagination } from '@appTypes/pagination'
import { User } from '@appTypes/user'
import { GroupMember } from '@appTypes/group'
import { Domain } from '@appTypes/domain'

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
    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/user/${id}/registration/domains/${domainId}`
    const apiResponse: any = await fetchJSON(
        apiUrl,
        setFetchOptions({ method: requestMethods.GET }),
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
    }
    serviceResponse.headers = headers
    return serviceResponse
}
