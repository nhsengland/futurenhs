import { setGetFetchOpts as setGetFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { getApiPaginationQueryParams } from '@helpers/routing/getApiPaginationQueryParams';
import { getClientPaginationFromApi } from '@helpers/routing/getClientPaginationFromApi';
import { FetchResponse } from '@appTypes/fetch';
import { ApiPaginatedResponse, ServicePaginatedResponse } from '@appTypes/service';
import { FolderContent } from '@appTypes/file';
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

export const getGroupFolderContents = async ({
    user,
    groupId,
    folderId,
    pagination
}: Options, dependencies?: Dependencies): Promise<ServicePaginatedResponse<Array<FolderContent>>> => {

    try {

        const serviceResponse: ServicePaginatedResponse<Array<FolderContent>> = {
            data: []
        };

        const setGetFetchOptions = dependencies?.setGetFetchOptions ?? setGetFetchOptionsHelper;
        const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

        const id: string = user.id;
        const paginationQueryParams: string = getApiPaginationQueryParams({ 
            pagination,
            defaults: {
                pageNumber: 1,
                pageSize: 30
            } 
        });

        const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/folders${folderId ? '/' + folderId + '/contents': ''}?${paginationQueryParams}`;
        const apiResponse: FetchResponse = await fetchJSON(apiUrl, setGetFetchOptions({}), 30000);
        const apiData: ApiPaginatedResponse<any> = apiResponse.json;
        const apiMeta: any = apiResponse.meta;

        const { ok, status, statusText } = apiMeta;

        if(!ok){

            return {
                errors: [{
                    [status]: statusText
                }]
            }

        }

        apiData.data?.forEach((datum) => {
            
            serviceResponse.data.push({
                id: datum.id ?? '',
                type: datum.type?.toLowerCase() ?? '',
                name: datum.name ?? '',
                createdBy: {
                    id: datum.firstRegistered?.by?.id ?? '',
                    text: {
                        userName: datum.firstRegistered?.by?.name ?? ''
                    }
                },
                modifiedBy: {
                    id: datum.lastUpdated?.by?.id ?? '',
                    text: {
                        userName: datum.lastUpdated?.by?.name ?? ''
                    }
                },
                modified: datum.lastUpdated?.atUtc ?? '',
                extension: datum.additionalMetadata?.fileExtension ?? '',
                text: {
                    body: datum.description ?? ''
                }
            });

        });

        serviceResponse.pagination = getClientPaginationFromApi({ apiPaginatedResponse: apiData });

        return serviceResponse;

    } catch(error){

        const { message } = error;

        return {
            errors: [{ error: message }],
        };

    }

}