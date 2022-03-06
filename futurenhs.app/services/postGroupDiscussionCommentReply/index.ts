import { setFetchOpts as setFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { services } from '@constants/services';
import { ServiceError } from '..';
import { Service, ServiceResponse } from '@appTypes/service';
import { User } from '@appTypes/user';

declare type Options = ({
    groupId: string;
    discussionId: string;
    commentId: string;
    user: User;
    csrfToken: string;
    body: FormData;
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

    const apiBase: string = typeof window !== 'undefined' ? process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL : process.env.NEXT_PUBLIC_API_BASE_URL;
    const apiUrl: string = `${apiBase}/v1/users/${id}/groups/${groupId}/discussions/${discussionId}/comments/${commentId}/replies`;

    const apiResponse: any = await fetchJSON(apiUrl, setFetchOptions({
        method: 'POST',
        body: {
            _csrf: body.get('_csrf'),
            Content: body.get(`content-${body.get('_instance-id')}`)
        }
    }), 30000);

    const apiMeta: any = apiResponse.meta;
    const apiData: any = apiResponse.json;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('Error posting new group discussion comment reply', {
            serviceId: services.POST_GROUP_DISCUSSION_COMMENT_REPLY,
            status: status,
            statusText: statusText,
            body: apiData
        });

    }

    return null;

}