import { setGetFetchOpts as setGetFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { getEnvVar } from '@helpers/util/env';
import { ServicePaginatedResponse } from '@appTypes/service';
import { Pagination } from '@appTypes/pagination';
import { SearchResult } from '@appTypes/search';
import { User } from '@appTypes/user';

declare type Options = ({
    user: User;
    term: string;
    pagination?: Pagination;
});

declare type Dependencies = ({
    setGetFetchOptions: any;
    fetchJSON: any;
});

export const getSearchResults = async ({
    user,
    term,
    pagination
}: Options, dependencies?: Dependencies): Promise<ServicePaginatedResponse<Array<SearchResult>>> => {

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
            data: [
                {
                    example: 'Result 1'
                },
                {
                    example: 'Result 2'
                },
                {
                    example: 'Result 3'
                }
            ]
        };

    } catch(error){

        const { message } = error;

        return {
            errors: { error: message },
        };

    }

}