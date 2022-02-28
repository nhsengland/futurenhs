import { setFetchOpts as setFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { ServiceError } from '..';
import { Service, ServiceResponse } from '@appTypes/service';
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
        content: string;
    }
});

declare type Dependencies = ({
    setFetchOptions: any;
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
    body
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<null>> => {

    const setFetchOptions = dependencies?.setFetchOptions ?? setFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

    const { id } = user;

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/discussions/${discussionId}/comments/${commentId}/replies`;

    const apiResponse: any = await fetchJSON(apiUrl, setFetchOptions({
        method: 'POST',
        body: {
            Content: body[`content-${body['_instance-id']}`]
        }
    }), 30000);

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