import { setGetFetchOpts as setGetFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { getApiPaginationQueryParams } from '@helpers/routing/getApiPaginationQueryParams';
import { getClientPaginationFromApi } from '@helpers/routing/getClientPaginationFromApi';
import { FetchResponse } from '@appTypes/fetch';
import { ApiPaginatedResponse, ServicePaginatedResponse } from '@appTypes/service';
import { Pagination } from '@appTypes/pagination';
import { SearchResult } from '@appTypes/search';

declare type Options = ({
    term: string;
    pagination?: Pagination;
});

declare type Dependencies = ({
    setGetFetchOptions: any;
    fetchJSON: any;
});

export const getSearchResults = async ({
    term,
    pagination
}: Options, dependencies?: Dependencies): Promise<ServicePaginatedResponse<Array<SearchResult>>> => {

    try {
        const serviceResponse: ServicePaginatedResponse<Array<any>> = {
            data: []
        };

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

        if (!ok) {

            return {
                errors: {
                    [status]: statusText
                }
            }

        }

        serviceResponse.data = apiData.data.results.map((item, index) => {

            if(item.type.match(/discussion-comment/gi)){

                return {
                    type: "comment",
                    entityIds: {
                        commentId: item.id
                    },
                    meta: {
                        type: "discussion",
                        entityIds: {
                            discussionId: item.id
                        },
                        meta: {
                            type: "group",
                            entityIds: {
                                groupId: item.group.slug
                            },
                            content: {
                                title: item.group.name,
                                body: item.group.description || "Auto generated group description"
                            }
                        },
                        content: {
                            title: item.name,
                            body: item.description || "Auto generated description"
                        }
                    },
                    content: {
                        title: item.name,
                        body: item.description || "Auto generated description"
                    }

                }

            }

            return {
                type: item.type,
                entityIds: {
                    [item.type+'Id']: item.id
                },
                meta:{
                    type: "group",
                    entityIds:{
                        groupId: item.group.slug
                    },
                    content: {
                        title: item.group.name,
                        body: item.group.description || "Auto populated description"
                    }
                },
                content: {
                    title: item.name,
                    body: item.description || "Auto populated description text"
                }
            }
        });

        serviceResponse.pagination = getClientPaginationFromApi({ apiPaginatedResponse: apiData });

        return serviceResponse;

    } catch (error) {

        const { message } = error;

        return {
            errors: { error: message },
        };

    }
}