import { setGetFetchOpts as setGetFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
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
    setGetFetchOptions: any;
    fetchJSON: any;
});

export const getGroupDiscussion = async ({
    groupId,
    discussionId,
    user
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<Discussion>> => {

    try {

        const serviceResponse: ServiceResponse<Discussion> = {
            data: null
        };

        const setGetFetchOptions = dependencies?.setGetFetchOptions ?? setGetFetchOptionsHelper;
        const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

        const { id } = user;

        const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/discussions/${discussionId}`;
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

    } catch(error){

        const { message } = error;

        return {
            errors: [{ error: message }],
        };

    }

}