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
import { SearchResult } from '@appTypes/search'
import jwtHeader from '@helpers/util/jwt/jwtHeader'
import { User } from '@appTypes/user'

declare type Options = {
    user: User
    term: string
    pagination?: Pagination
    minLength?: Number
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const getSearchResults = async (
    { user, term, pagination, minLength }: Options,
    dependencies?: Dependencies
): Promise<ServicePaginatedResponse<Array<SearchResult>>> => {
    const serviceResponse: ServicePaginatedResponse<Array<any>> = {
        data: [],
    }

    if (!term || term.length < minLength) {
        return serviceResponse
    }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const paginationQueryParams: string = getApiPaginationQueryParams({
        pagination,
        defaults: {
            pageNumber: 1,
            pageSize: 30,
        },
    })

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/search?term=${term}&${paginationQueryParams}`
    const authHeader = jwtHeader(user.accessToken)
    const apiHeaders = setFetchOptions({
        method: requestMethods.GET,
        headers: authHeader,
    })
    const apiResponse: FetchResponse = await fetchJSON(
        apiUrl,
        apiHeaders,
        defaultTimeOutMillis
    )

    const apiData: ApiPaginatedResponse<any> = apiResponse.json
    const apiMeta: any = apiResponse.meta

    const { ok, status, statusText } = apiMeta

    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to get the search results',
            {
                serviceId: services.GET_SEARCH_RESULTS,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    serviceResponse.data = apiData.data.results.map((item, index) => {
        if (item.type.match(/discussion-comment/gi)) {
            return {
                type: 'comment',
                entityIds: {
                    commentId: item.id,
                },
                meta: {
                    type: 'discussion',
                    entityIds: {
                        discussionId: item.id,
                    },
                    meta: {
                        type: 'group',
                        entityIds: {
                            groupId: item.group.slug,
                        },
                        content: {
                            title: item.group.name,
                            body: item.group.description,
                        },
                    },
                    content: {
                        title: item.name,
                        body: item.description,
                    },
                },
                content: {
                    title: item.name,
                    body: item.description,
                },
            }
        }

        return {
            type: item.type,
            entityIds: {
                [item.type + 'Id']: item.id,
            },
            meta: {
                type: 'group',
                entityIds: {
                    groupId: item.group.slug,
                },
                content: {
                    title: item.group.name,
                    body: item.group.description,
                },
            },
            content: {
                title: item.name,
                body: item.description,
            },
        }
    })

    serviceResponse.pagination = getClientPaginationFromApi({
        apiPaginatedResponse: apiData,
    })

    return serviceResponse
}
