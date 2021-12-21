import { setGetFetchOpts as setGetFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { ServicePaginatedResponse } from '@appTypes/service';
import { Pagination } from '@appTypes/pagination';
import { User } from '@appTypes/user';
import { GroupMember } from '@appTypes/group';

import { mockMemberData } from './mockMemberData';

declare type Options = ({
    user: User;
    slug: string;
    pagination?: Pagination;
});

declare type Dependencies = ({
    setGetFetchOptions: any;
    fetchJSON: any;
});

export const getGroupMembers = async ({
    user,
    slug,
    pagination
}: Options, dependencies?: Dependencies): Promise<ServicePaginatedResponse<Array<GroupMember>>> => {

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

        const { pageNumber, pageSize } = pagination;

        const sortedMembers: Array<GroupMember> = mockMemberData.sort((a, b) => a.role.localeCompare(b.role));
        const mockData: Array<GroupMember> = sortedMembers.map((item) => item);

        const lowerBound: number = pageNumber === 1 ? 0 : (pageNumber - 1) * pageSize;
        const upperBound: number = pageNumber === 1 ? pageSize : pageNumber * pageSize;

        const json = {
            data: mockData.slice(lowerBound, upperBound),
            pagination: {
                pageNumber: pageNumber,
                pageSize: pageSize,
                totalRecords: mockData.length
            }
        };

        return json;

    } catch(error){

        const { message } = error;

        return {
            errors: { error: message },
        };

    }

}