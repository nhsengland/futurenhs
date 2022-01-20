import { setGetFetchOpts as setGetFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { getEnvVar } from '@helpers/util/env';
import { getApiPaginationQueryParams } from '@helpers/routing/getApiPaginationQueryParams';
import { getClientPaginationFromApi } from '@helpers/routing/getClientPaginationFromApi';
import { FetchResponse } from '@appTypes/fetch';
import { ApiPaginatedResponse, ServicePaginatedResponse } from '@appTypes/service';
import { Pagination } from '@appTypes/pagination';
import { SearchResult } from '@appTypes/search';
import { User } from '@appTypes/user';

declare type Options = ({
    user: User;
    term: string;
    pagination?: Pagination;
});

declare type Dependencies = ({
    setGetFetchOptions: any;
    fetchJSON: any;
});

export const getSearchResults = async ({
    user,
    term,
    pagination
}: Options, dependencies?: Dependencies): Promise<ServicePaginatedResponse<Array<SearchResult>>> => {

    try {
        const serviceResponse: ServicePaginatedResponse<Array<any>> = {
            data: []
        };

        const setGetFetchOptions = dependencies?.setGetFetchOptions ?? setGetFetchOptionsHelper;
        const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

        const paginationQueryParams: string = getApiPaginationQueryParams({ pagination });

        const apiUrl: string = `${getEnvVar({ name: 'NEXT_PUBLIC_API_BASE_URL' })}/v1/search?term=${term}&${paginationQueryParams}`;
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


        // TODO - this is a basic mapping example //map the actual data

        // data[(apiData.offset + index) % data.length]
        serviceResponse.data = apiData.data.results.map((item, index) => {
            // console.log(item)

            if(item.type.match(/discussion-comment/gi)){
                console.log("discussion-comment: ", item)
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

        console.log(serviceResponse.data)

        serviceResponse.pagination = getClientPaginationFromApi({ apiPaginatedResponse: apiData });
        return serviceResponse;

    } catch (error) {

        const { message } = error;

        return {
            errors: { error: message },
        };

    }
}