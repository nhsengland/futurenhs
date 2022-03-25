import { setFetchOpts as setFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { services } from '@constants/services';
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch';
import { ServiceError } from '..';
import { getApiPaginationQueryParams } from '@helpers/routing/getApiPaginationQueryParams';
import { getClientPaginationFromApi } from '@helpers/routing/getClientPaginationFromApi';
import { ServicePaginatedResponse } from '@appTypes/service';
import { Pagination } from '@appTypes/pagination';
import { FetchResponse } from '@appTypes/fetch';
import { ApiResponse } from '@appTypes/service';
import { Discussion } from '@appTypes/discussion';
import { User } from '@appTypes/user';

declare type Options = ({
    groupId: string;
    user: User;
    pagination?: Pagination;
});

declare type Dependencies = ({
    setFetchOptions: any;
    fetchJSON: any;
});

export const getGroupDiscussions = async ({
    groupId,
    user,
    pagination
}: Options, dependencies?: Dependencies): Promise<ServicePaginatedResponse<Array<Discussion>>> => {

    const serviceResponse: ServicePaginatedResponse<Array<Discussion>> = {
        data: []
    };

    const setFetchOptions = dependencies?.setFetchOptions ?? setFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

    const { id } = user;
    const paginationQueryParams: string = getApiPaginationQueryParams({
        pagination,
        defaults: {
            pageNumber: 1,
            pageSize: 30
        }
    });

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/discussions?${paginationQueryParams}`;
    const apiResponse: FetchResponse = await fetchJSON(apiUrl, setFetchOptions({ method: requestMethods.GET }), defaultTimeOutMillis);
    const apiData: ApiResponse<any> = apiResponse.json;
    const apiMeta: any = apiResponse.meta;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('Error getting group discussions', {
            serviceId: services.GET_GROUP_DISCUSSIONS,
            status: status,
            statusText: statusText,
            body: apiData
        });

    }

    apiData.data?.forEach((datum) => {

        serviceResponse.data.push({
            text: {
                title: datum.title ?? null
            },
            discussionId: datum.id,
            responseCount: datum.totalComments ?? 0,
            viewCount: datum.views ?? 0,
            createdBy: {
                id: datum.firstRegistered?.by?.id ?? '',
                text: {
                    userName: datum.firstRegistered?.by?.name ?? ''
                }
            },
            created: datum.firstRegistered?.atUtc ?? '',
            modifiedBy: {
                id: datum.lastComment?.by?.id ?? '',
                text: {
                    userName: datum.lastComment?.by?.name ?? ''
                }
            },
            modified: datum.lastComment?.atUtc ?? '',
            isSticky: datum.isSticky
        });

    });

    serviceResponse.pagination = getClientPaginationFromApi({ apiPaginatedResponse: apiData });

    return serviceResponse;

}