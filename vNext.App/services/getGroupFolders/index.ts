import { setGetFetchOpts as setGetFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { getApiPaginationQueryParams } from '@helpers/routing/getApiPaginationQueryParams';
import { getClientPaginationFromApi } from '@helpers/routing/getClientPaginationFromApi';
import { FetchResponse } from '@appTypes/fetch';
import { ApiPaginatedResponse, ServicePaginatedResponse } from '@appTypes/service';
import { Folder } from '@appTypes/file';
import { User } from '@appTypes/user';

declare type Options = ({
    user: User;
    groupId: string;
    folderId?: string;
    pagination: {
        pageNumber?: number;
        pageSize?: number;
    };
});

declare type Dependencies = ({
    setGetFetchOptions: any;
    fetchJSON: any;
});

export const getGroupFolders = async ({
    user,
    groupId,
    folderId,
    pagination
}: Options, dependencies?: Dependencies): Promise<ServicePaginatedResponse<Array<Folder>>> => {

    try {

        const serviceResponse: ServicePaginatedResponse<Array<Folder>> = {
            data: []
        };

        const setGetFetchOptions = dependencies?.setGetFetchOptions ?? setGetFetchOptionsHelper;
        const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

        const id: string = user.id;
        const paginationQueryParams: string = getApiPaginationQueryParams({ pagination });

        const apiUrl: string = `${process.env.NEXT_PUBLIC_API_BASE_URL}/v1/users/${id}/groups/${groupId}/folders${folderId ? '/' + folderId + '/contents': ''}?${paginationQueryParams}`;
        const apiResponse: FetchResponse = await fetchJSON(apiUrl, setGetFetchOptions({}), 30000);
        const apiData: ApiPaginatedResponse<any> = apiResponse.json;
        const apiMeta: any = apiResponse.meta;

        const { ok, status, statusText } = apiMeta;

        if(!ok){

            return {
                errors: {
                    [status]: statusText
                }
            }

        }

        apiData.data?.forEach((datum) => {

            serviceResponse.data.push({
                id: datum.id ?? '',
                type: datum.type?.toLowerCase() ?? '',
                name: datum.name ?? '',
                bodyHtml: datum.description ?? '',
                modified: datum.lastUpdated ?? ''
            });

        });

        serviceResponse.pagination = getClientPaginationFromApi({ apiPaginatedResponse: apiData });

        return serviceResponse;

    } catch(error){

        const { message } = error;

        return {
            errors: { error: message },
        };

    }

}