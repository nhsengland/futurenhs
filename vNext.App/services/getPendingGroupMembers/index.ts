import { setGetFetchOpts as setGetFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { getApiPaginationQueryParams } from '@helpers/routing/getApiPaginationQueryParams';
import { getClientPaginationFromApi } from '@helpers/routing/getClientPaginationFromApi';
import { FetchResponse } from '@appTypes/fetch';
import { ApiPaginatedResponse, ServicePaginatedResponse } from '@appTypes/service';
import { Pagination } from '@appTypes/pagination';
import { User } from '@appTypes/user';
import { GroupMember } from '@appTypes/group';

declare type Options = ({
    user: User;
    groupId: string;
    pagination?: Pagination;
});

declare type Dependencies = ({
    setGetFetchOptions: any;
    fetchJSON: any;
});

export const getPendingGroupMembers = async ({
    user,
    groupId,
    pagination
}: Options, dependencies?: Dependencies): Promise<ServicePaginatedResponse<Array<GroupMember>>> => {

    try {

        const serviceResponse: ServicePaginatedResponse<Array<GroupMember>> = {
            data: []
        };

        const setGetFetchOptions = dependencies?.setGetFetchOptions ?? setGetFetchOptionsHelper;
        const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

        const id: string = user.id;
        const paginationQueryParams: string = getApiPaginationQueryParams({ pagination });

        const apiUrl: string = `${process.env.NEXT_PUBLIC_API_BASE_URL}/v1/users/${id}/groups/${groupId}/members/pending?${paginationQueryParams}`;
        const apiResponse: FetchResponse = await fetchJSON(apiUrl, setGetFetchOptions({}), 30000);
        const apiData: ApiPaginatedResponse<any> = apiResponse.json;
        const apiMeta: any = apiResponse.meta;

        const { ok, status, statusText } = apiMeta;

        if(!ok){

            return {
                errors: {
                    [status]: statusText
                }
            }

        }


        apiData.data?.forEach((datum) => {

            serviceResponse.data.push({
                id: datum.id ?? '',
                fullName: datum.name ?? '',
                email: '',
                role: datum.role ?? '',
                joinDate: datum.dateJoinedUtc ?? '', 
                lastLogInDate: datum.lastLoginUtc ?? ''
            });

        });

        serviceResponse.pagination = getClientPaginationFromApi({ apiPaginatedResponse: apiData });

        return serviceResponse;

    } catch(error){

        const { message } = error;

        return {
            errors: { error: message },
        };

    }

}