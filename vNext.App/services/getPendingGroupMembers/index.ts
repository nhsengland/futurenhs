import { setGetFetchOpts as setGetFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { ServiceResponse } from '@appTypes/service';
import { User } from '@appTypes/user';
import { GroupMember } from '@appTypes/group';

import { mockPendingMemberData } from './mockMemberData';

declare type Options = ({
    user: User;
    slug: string;
});

declare type Dependencies = ({
    setGetFetchOptions: any;
    fetchJSON: any;
});

export const getPendingGroupMembers = async ({
    user,
    slug
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<Array<GroupMember>>> => {

    try {

        // const setGetFetchOptions = dependencies?.setGetFetchOptions ?? setGetFetchOptionsHelper;
        // const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

        // const id: string = user.id;
        // const apiUrl: string = `${getEnvVar({ name: 'NEXT_PUBLIC_API_BASE_URL' })}/v1/users/${id}/groups/${slug}/members`;
        // const { json, meta } = await fetchJSON(apiUrl, setGetFetchOptions({}), 30000);
        // const { ok, status, statusText } = meta;

        // if(!ok){

        //     return {
        //         errors: {
        //             [status]: statusText
        //         }
        //     }

        // }

        const json = {
            data: mockPendingMemberData
        };

        return json;

    } catch(error){

        const { message } = error;

        return {
            errors: { error: message },
        };

    }

}