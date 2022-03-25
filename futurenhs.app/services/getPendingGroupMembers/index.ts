import { setFetchOpts as setFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { services } from '@constants/services';
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch';
import { ServiceError } from '..';
import { getApiPaginationQueryParams } from '@helpers/routing/getApiPaginationQueryParams';
import { getClientPaginationFromApi } from '@helpers/routing/getClientPaginationFromApi';
import { FetchResponse } from '@appTypes/fetch';
import { Service, ApiPaginatedResponse, ServicePaginatedResponse } from '@appTypes/service';
import { Pagination } from '@appTypes/pagination';
import { User } from '@appTypes/user';
import { GroupMember } from '@appTypes/group';

declare type Options = ({
    user: User;
    groupId: string;
    pagination?: Pagination;
});

declare type Dependencies = ({
    setFetchOptions: any;
    fetchJSON: any;
});

export const getPendingGroupMembers: Service = async ({
    user,
    groupId,
    pagination
}: Options, dependencies?: Dependencies): Promise<ServicePaginatedResponse<Array<GroupMember>>> => {

    const serviceResponse: ServicePaginatedResponse<Array<GroupMember>> = {
        data: []
    };

    const setFetchOptions = dependencies?.setFetchOptions ?? setFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

    const id: string = user.id;
    const paginationQueryParams: string = getApiPaginationQueryParams({
        pagination,
        defaults: {
            pageNumber: 1,
            pageSize: 10
        }
    });

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/members/pending?${paginationQueryParams}`;
    const apiResponse: FetchResponse = await fetchJSON(apiUrl, setFetchOptions({ method: requestMethods.GET }), defaultTimeOutMillis);
    
    const apiData: ApiPaginatedResponse<any> = apiResponse.json;
    const apiMeta: any = apiResponse.meta;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('Error getting pending group members', {
            serviceId: services.GET_PENDING_GROUP_MEMBERS,
            status: status,
            statusText: statusText,
            body: apiData
        });

    }

    apiData.data?.forEach((datum) => {

        serviceResponse.data.push({
            id: datum.id ?? '',
            fullName: datum.name ?? '',
            email: datum.email ?? '',
            requestDate: datum.applicationDateUtc ?? '',
        });

    });

    serviceResponse.pagination = getClientPaginationFromApi({ apiPaginatedResponse: apiData });

    return serviceResponse;

}