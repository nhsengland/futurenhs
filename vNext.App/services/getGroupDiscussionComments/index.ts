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
        const paginationQueryParams: string = getApiPaginationQueryParams({ 
            pagination,
            defaults: {
                pageNumber: 1,
                pageSize: 30
            }
        });

        const apiUrl: string = `${process.env.NEXT_PUBLIC_API_BASE_URL}/v1/users/${id}/groups/${groupId}/discussions/${discussionId}/comments?${paginationQueryParams}`;
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

            console.log(datum)
              

            // {
            //     id: '60ac1c59-0344-4f8a-8568-ada50107c0e4',
            //     content: '<p>fggfdgfdgsg</p>',
            //     repliesCount: 0,
            //     likesCount: 0,
            //     firstRegistered: {
            //       atUtc: '2021-09-16T16:00:17Z',
            //       by: {
            //         id: 'f7a521aa-2746-4507-b50f-ad4000fd15ff',
            //         name: 'John Waters',
            //         slug: 'johnnyw'
            //       }
            //     },
            //     currentUser: { created: false, liked: false }
            //   }

            // createdBy: datum.firstRegistered?.by?.name ?? '',
            // modifiedBy: datum.lastUpdated?.by?.name ?? '',
            // modified: datum.lastUpdated?.atUtc ?? '',
              

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