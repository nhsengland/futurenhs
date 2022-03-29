import { setFetchOpts as setFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { cacheNames } from '@constants/caches';
import { clearClientCaches } from '@helpers/util/data';
import { services } from '@constants/services';
import { requestMethods, defaultTimeOutMillis } from '@constants/fetch';
import { ServiceError } from '..';
import { Service, ServiceResponse } from '@appTypes/service';
import { User } from '@appTypes/user';

declare type Options = ({
    groupId: string;
    discussionId: string;
    user: User;
    headers: any;
    body: FormData;
});

declare type Dependencies = ({
    setFetchOptions: any;
    fetchJSON: any;
});

/**
 * Posts a new group discussion comment
 */
export const postGroupDiscussionComment: Service = async ({
    groupId,
    discussionId,
    user,
    headers,
    body
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<null>> => {

    const setFetchOptions = dependencies?.setFetchOptions ?? setFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

    const { id } = user;

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/discussions/${discussionId}/comments`;
    const apiResponse: any = await fetchJSON(apiUrl, setFetchOptions({
        method: requestMethods.POST,
        headers: headers,
        body: {
            content: body.get('content')
        }
    }), defaultTimeOutMillis);

    const apiMeta: any = apiResponse.meta;
    const apiData: any = apiResponse.json;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('Error posting new group discussion', {
            serviceId: services.POST_GROUP_DISCUSSION_COMMENT,
            status: status,
            statusText: statusText,
            body: apiData
        });

    }

    await clearClientCaches([cacheNames.NEXT_DATA]);

    return null;

}