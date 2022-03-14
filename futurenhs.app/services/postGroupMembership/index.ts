import { setFetchOpts as setFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { services } from '@constants/services';
import { requestMethods } from '@constants/fetch';
import { ServiceError } from '..';
import { FetchResponse } from '@appTypes/fetch';
import { ApiResponse, ServiceResponse } from '@appTypes/service';
import { Group } from '@appTypes/group';
import { User } from '@appTypes/user';

declare type Options = ({
    csrfToken: string;
    groupId: string;
    user: User;
});

declare type Dependencies = ({
    setFetchOptions: any;
    fetchJSON: any;
});

export type PostGroupMembershipService = (options: Options, dependencies?: Dependencies) => Promise<ServiceResponse<Group>>;

export const postGroupMembership = async ({
    csrfToken,
    groupId,
    user
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<Group>> => {

    const setFetchOptions = dependencies?.setFetchOptions ?? setFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

    const { id } = user;

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_BASE_URL}/v1/users/${id}/groups/${groupId}/members/join`;
    const apiResponse: FetchResponse = await fetchJSON(apiUrl, setFetchOptions({
        method: requestMethods.POST,
        body: {
            _csrf: csrfToken
        }
    }), 30000);

    const apiData: ApiResponse<any> = apiResponse.json;
    const apiMeta: any = apiResponse.meta;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('Error posting group membership', {
            serviceId: services.POST_GROUP_MEMBERSHIP,
            status: status,
            statusText: statusText,
            body: apiData
        });

    }

    return {};

}