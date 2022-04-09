import { setFetchOpts as setFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { services } from '@constants/services';
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch';
import { ServiceError } from '..';
import { ServiceResponse } from '@appTypes/service';
import { FetchResponse } from '@appTypes/fetch';
import { ApiResponse } from '@appTypes/service';
import { Discussion } from '@appTypes/discussion';
import { User } from '@appTypes/user';

declare type Options = ({
    groupId: string;
    discussionId: string;
    user: User;
});

declare type Dependencies = ({
    setFetchOptions: any;
    fetchJSON: any;
});

export const getGroupDiscussion = async ({
    groupId,
    discussionId,
    user
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<Discussion>> => {

    const serviceResponse: ServiceResponse<Discussion> = {
        data: null
    };

    const setFetchOptions = dependencies?.setFetchOptions ?? setFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

    const { id } = user;

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/discussions/${discussionId}`;
    const apiResponse: FetchResponse = await fetchJSON(apiUrl, setFetchOptions({ method: requestMethods.GET }), defaultTimeOutMillis);
    
    const apiData: ApiResponse<any> = apiResponse.json;
    const apiMeta: any = apiResponse.meta;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('An unexpected error occurred when attempting to get the group discussion', {
            serviceId: services.GET_GROUP_DISCUSSION,
            status: status,
            statusText: statusText,
            body: apiData
        });

    }

    serviceResponse.data = {
        text: {
            title: apiData?.title,
            body: apiData?.description
        },
        responseCount: apiData.totalComments ?? 0,
        viewCount: apiData.views ?? 0,
        createdBy: {
            id: apiData.firstRegistered?.by?.id ?? '',
            text: {
                userName: apiData.firstRegistered?.by?.name ?? ''
            }
        },
        created: apiData.firstRegistered?.atUtc ?? '',
        modifiedBy: {
            id: apiData.lastComment?.by?.id ?? '',
            text: {
                userName: apiData.lastComment?.by?.name ?? ''
            }
        },
        modified: apiData.lastComment?.atUtc ?? '',
    };

    return serviceResponse;

}