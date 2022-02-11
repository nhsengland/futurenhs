import { setGetFetchOpts as setGetFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { ServiceError } from '..';
import { getApiPaginationQueryParams } from '@helpers/routing/getApiPaginationQueryParams';
import { getClientPaginationFromApi } from '@helpers/routing/getClientPaginationFromApi';
import { FetchResponse } from '@appTypes/fetch';
import { ApiPaginatedResponse, ServicePaginatedResponse } from '@appTypes/service';
import { Pagination } from '@appTypes/pagination';
import { SearchResult } from '@appTypes/search';

declare type Options = ({
    term: string;
    pagination?: Pagination;
    minLength?: Number
});

declare type Dependencies = ({
    setGetFetchOptions: any;
    fetchJSON: any;
});

export const getSearchResults = async ({
    term,
    pagination,
    minLength
}: Options, dependencies?: Dependencies): Promise<ServicePaginatedResponse<Array<SearchResult>>> => {

    const serviceResponse: ServicePaginatedResponse<Array<any>> = {
        data: []
    };

    if (term && term.length < minLength) return serviceResponse;

    const setGetFetchOptions = dependencies?.setGetFetchOptions ?? setGetFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

    const paginationQueryParams: string = getApiPaginationQueryParams({
        pagination,
        defaults: {
            pageNumber: 1,
            pageSize: 30
        }
    });

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/search?term=${term}&${paginationQueryParams}`;
    const apiResponse: FetchResponse = await fetchJSON(apiUrl, setGetFetchOptions({}), 30000);
    const apiData: ApiPaginatedResponse<any> = apiResponse.json;
    const apiMeta: any = apiResponse.meta;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('Error getting search results', {
            status: status,
            statusText: statusText,
            body: apiData
        });

    }

    serviceResponse.data = apiData.data.results.map((item, index) => {

        if (item.type.match(/discussion-comment/gi)) {

            return {
                type: 'comment',
                entityIds: {
                    commentId: item.id
                },
                meta: {
                    type: 'discussion',
                    entityIds: {
                        discussionId: item.id
                    },
                    meta: {
                        type: 'group',
                        entityIds: {
                            groupId: item.group.slug
                        },
                        content: {
                            title: item.group.name,
                            body: item.group.description
                        }
                    },
                    content: {
                        title: item.name,
                        body: item.description
                    }
                },
                content: {
                    title: item.name,
                    body: item.description
                }

            }

        }

        return {
            type: item.type,
            entityIds: {
                [item.type + 'Id']: item.id
            },
            meta: {
                type: 'group',
                entityIds: {
                    groupId: item.group.slug
                },
                content: {
                    title: item.group.name,
                    body: item.group.description
                }
            },
            content: {
                title: item.name,
                body: item.description
            }
        }

    });

    serviceResponse.pagination = getClientPaginationFromApi({ apiPaginatedResponse: apiData });

    return serviceResponse;

}