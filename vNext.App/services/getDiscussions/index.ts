import { setGetFetchOpts as setGetFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { getEnvVar } from '@helpers/util/env';
import { ServicePaginatedResponse } from '@appTypes/service';
import { Pagination } from '@appTypes/pagination';
import { Group } from '@appTypes/group';
import { User } from '@appTypes/user';

declare type Options = ({
    user: User;
    pagination?: Pagination;
});

declare type Dependencies = ({
    setGetFetchOptions: any;
    fetchJSON: any;
});

export const getDiscussions = async ({
    user,
    pagination
}: Options, dependencies?: Dependencies): Promise<ServicePaginatedResponse<Array<Group>>> => {

    try {

        // const setGetFetchOptions = dependencies?.setGetFetchOptions ?? setGetFetchOptionsHelper;
        // const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

        // const { pageNumber, pageSize } = pagination;

        // const id: string = user.id;
        // const apiUrl: string = `${getEnvVar({ name: 'NEXT_PUBLIC_API_BASE_URL' })}/v1/users/${id}/${resource}?pageNumber=${pageNumber}&pageSize=${pageSize}`;

        // const { json, meta } = await fetchJSON(apiUrl, setGetFetchOptions({}), 30000);
        // const { ok, status, statusText } = meta;

        // if(!ok){

        //     return {
        //         errors: {
        //             [status]: statusText
        //         }
        //     }

        // }

        return {
            data: []
        };

    } catch(error){

        const { message } = error;

        return {
            errors: { error: message },
        };

    }

}