import { setGetFetchOpts as setGetFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { FetchResponse } from '@appTypes/fetch';
import { ApiResponse, ServiceResponse } from '@appTypes/service';
import { User } from '@appTypes/user';
import { GroupMember } from '@appTypes/group';

declare type Options = ({
    user: User;
    groupId: string;
    memberId: string;
});

declare type Dependencies = ({
    setGetFetchOptions: any;
    fetchJSON: any;
});

export const getGroupMember = async ({
    user,
    groupId,
    memberId
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<GroupMember>> => {

    try {

        const serviceResponse: ServiceResponse<GroupMember> = {
            data: null
        };

        const setGetFetchOptions = dependencies?.setGetFetchOptions ?? setGetFetchOptionsHelper;
        const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

        const id: string = user.id;

        const apiUrl: string = `${process.env.NEXT_PUBLIC_API_BASE_URL}/v1/users/${id}/groups/${groupId}/members/${memberId}`;
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
            id: apiData.id ?? '',
            firstName: apiData.firstName ?? '',
            lastName: apiData.lastName ?? '',
            email: apiData.email ?? '',
            pronouns: apiData.pronouns ?? '',
            role: apiData.role ?? '',
            joinDate: apiData.dateJoinedUtc ?? '', 
            lastLogInDate: apiData.lastLoginUtc ?? ''
        };

        return serviceResponse;

    } catch(error){

        const { message } = error;

        return {
            errors: [{ error: message }],
        };

    }

}