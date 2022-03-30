import { setFetchOpts as setFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { services } from '@constants/services';
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch';
import { ServiceError } from '..';
import { FetchResponse } from '@appTypes/fetch';
import { ApiResponse, ServiceResponse } from '@appTypes/service';
import { User } from '@appTypes/user';

declare type Options = ({
    groupId: string;
    discussionId: string;
    commentId: string;
    user: User;
    shouldLike: boolean;
});

declare type Dependencies = ({
    setFetchOptions: any;
    fetchJSON: any;
});

export const putGroupDiscussionCommentLike = async ({
    groupId,
    discussionId,
    commentId,
    user,
    shouldLike,
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<null>> => {

    const setFetchOptions = dependencies?.setFetchOptions ?? setFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

    const { id } = user;

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/discussions/${discussionId}/comments/${commentId}/${shouldLike ? 'like' : 'unlike'}`;
    const apiResponse: FetchResponse = await fetchJSON(apiUrl, setFetchOptions({ method: requestMethods.PUT }), defaultTimeOutMillis);
    
    const apiData: ApiResponse<any> = apiResponse.json;
    const apiMeta: any = apiResponse.meta;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('Error putting group discussion comment like', {
            serviceId: services.PUT_GROUP_DISCUSSION_COMMENT_LIKE,
            status: status,
            statusText: statusText,
            body: apiData
        });

    }

    return null;

}