import { setPostFetchOpts as setPostFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { ServiceError } from '..';
import { Service, ServiceResponse } from '@appTypes/service';
import { FetchResponse } from '@appTypes/fetch';
import { User } from '@appTypes/user';

declare type Options = ({
    groupId: string;
    discussionId: string;
    commentId: string;
    user: User;
    csrfToken: string;
    body: {
        _csrf: string;
        formId: string;
        title: string;
        comment: string;
    }
});

declare type Dependencies = ({
    setPostFetchOptions: any;
    fetchJSON: any;
});

/**
 * Posts a new group discussion comment
 */
export const postGroupDiscussionCommentReply: Service = async ({
    groupId,
    discussionId,
    commentId,
    user,
    csrfToken,
    body
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<null>> => {

    const setPostFetchOptions = dependencies?.setPostFetchOptions ?? setPostFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

    const { id } = user;

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/discussions/${discussionId}/comments/${commentId}`;

    const apiResponse: any = await fetchJSON(apiUrl, setPostFetchOptions({}, body), 30000);
    const apiMeta: any = apiResponse.meta;
    const apiData: any = apiResponse.json;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('Error posting new group discussion comment reply', {
            status: status,
            statusText: statusText,
            body: apiData
        });

    }

    return null;

}