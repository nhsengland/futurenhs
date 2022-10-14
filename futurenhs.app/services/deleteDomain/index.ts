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
} from '@appTypes/service'
import { Pagination } from '@appTypes/pagination'
import { User } from '@appTypes/user'
import { GroupMember } from '@appTypes/group'
import { Domain } from '@appTypes/domain'
import { api } from '@constants/routes'

declare type Options = {
    headers?: any
    user: User
    domain: string
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const deleteDomain = async (
    { headers, user, domain }: Options,
    dependencies?: Dependencies
): Promise<ServicePaginatedResponse<Array<Domain>>> => {
    const serviceResponse: ServicePaginatedResponse<Array<Domain>> = {
        data: [],
    }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper
    console.log(domain)
    const id: string = user.id
    const domainPath = api.ALLOW_DOMAIN.replace('%USER_ID%', id)
    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}${domainPath}`
    const apiResponse: any = await fetchJSON(
        apiUrl,
        setFetchOptions({
            method: requestMethods.POST,
            headers: headers,
            body: {
                EmailDomain: domain,
            },
        }),
        defaultTimeOutMillis
    )
    console.log(JSON.stringify(apiResponse))
    const apiData: ApiPaginatedResponse<any> = apiResponse.json
    const apiMeta: any = apiResponse.meta

    const { ok, status, statusText } = apiMeta
    debugger
    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to get the approved domains',
            {
                serviceId: services.GET_DOMAINS,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    apiData.data?.forEach((datum) => {
        serviceResponse.data.push({
            dateAdded: datum.id,
            domain: datum.emailDomain,
        })
    })

    serviceResponse.pagination = getClientPaginationFromApi({
        apiPaginatedResponse: apiData,
    })

    return serviceResponse
}
