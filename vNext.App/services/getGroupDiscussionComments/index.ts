import { setGetFetchOpts as setGetFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { getApiPaginationQueryParams } from '@helpers/routing/getApiPaginationQueryParams';
import { getClientPaginationFromApi } from '@helpers/routing/getClientPaginationFromApi';
import { ServicePaginatedResponse } from '@appTypes/service';
import { FetchResponse } from '@appTypes/fetch';
import { ApiResponse } from '@appTypes/service';
import { Discussion } from '@appTypes/discussion';
import { User } from '@appTypes/user';
import { Pagination } from '@appTypes/pagination';

declare type Options = ({
    groupId: string;
    discussionId: string;
    user: User;
    pagination: Pagination;
});

declare type Dependencies = ({
    setGetFetchOptions: any;
    fetchJSON: any;
});

export const getGroupDiscussionComments = async ({
    groupId,
    discussionId,
    user,
    pagination
}: Options, dependencies?: Dependencies): Promise<ServicePaginatedResponse<Array<Discussion>>> => {

    try {

        const serviceResponse: ServicePaginatedResponse<Array<Discussion>> = {
            data: []
        };

        const setGetFetchOptions = dependencies?.setGetFetchOptions ?? setGetFetchOptionsHelper;
        const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

        const { id } = user;
        const paginationQueryParams: string = getApiPaginationQueryParams({ pagination });

        const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/discussions/${discussionId}/comments?${paginationQueryParams}`;
        const apiResponse: FetchResponse = await fetchJSON(apiUrl, setGetFetchOptions({}), 30000);
        const apiData: ApiResponse<any> = apiResponse.json;
        const apiMeta: any = apiResponse.meta;

        const { ok, status, statusText } = apiMeta;

        if(!ok){

            return {
                errors: [{
                    [status]: statusText
                }]
            }

        }

        apiData.data?.forEach((datum) => {

            serviceResponse.data.push(datum);

        });

        serviceResponse.pagination = getClientPaginationFromApi({ apiPaginatedResponse: apiData });

        return serviceResponse;

    } catch(error){

        const { message } = error;

        return {
            errors: [{ error: message }],
        };

    }

}