import { setPostFetchOpts as setPostFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { ServiceError } from '..';
import { Service, ServiceResponse } from '@appTypes/service';
import { User } from '@appTypes/user';

declare type Options = ({
    groupId: string;
    discussionId: string;
    user: User;
    csrfToken: string;
    body: {
        _csrf: string;
        formId: string;
        content: string;
    }
});

declare type Dependencies = ({
    setPostFetchOptions: any;
    fetchJSON: any;
});

/**
 * Posts a new group discussion comment
 */
export const postGroupDiscussionComment: Service = async ({
    groupId,
    discussionId,
    user,
    body
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<null>> => {

    const setPostFetchOptions = dependencies?.setPostFetchOptions ?? setPostFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

    const { id } = user;

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/discussions/${discussionId}/comments`;

    const apiResponse: any = await fetchJSON(apiUrl, setPostFetchOptions({}, {
        content: body.content
    }), 30000);

    const apiMeta: any = apiResponse.meta;
    const apiData: any = apiResponse.json;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('Error posting new group discussion', {
            status: status,
            statusText: statusText,
            body: apiData
        });

    }

    return null;

}