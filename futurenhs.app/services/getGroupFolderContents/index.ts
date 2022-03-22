import { setFetchOpts as setFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { services } from '@constants/services';
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch';
import { ServiceError } from '..';
import { getApiPaginationQueryParams } from '@helpers/routing/getApiPaginationQueryParams';
import { getClientPaginationFromApi } from '@helpers/routing/getClientPaginationFromApi';
import { getGroupFileDownload } from '@services/getGroupFileDownload';
import { FetchResponse } from '@appTypes/fetch';
import { ApiPaginatedResponse, ServicePaginatedResponse, ServiceResponse } from '@appTypes/service';
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
    setFetchOptions: any;
    fetchJSON: any;
});

export const getGroupFolderContents = async ({
    user,
    groupId,
    folderId,
    pagination
}: Options, dependencies?: Dependencies): Promise<ServicePaginatedResponse<Array<FolderContent>>> => {

    const serviceResponse: ServicePaginatedResponse<Array<FolderContent>> = {
        data: []
    };

    const setFetchOptions = dependencies?.setFetchOptions ?? setFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

    const id: string = user.id;
    const paginationQueryParams: string = getApiPaginationQueryParams({
        pagination,
        defaults: {
            pageNumber: 1,
            pageSize: 30
        }
    });

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/folders${folderId ? '/' + folderId + '/contents' : ''}?${paginationQueryParams}`;
    const apiResponse: FetchResponse = await fetchJSON(apiUrl, setFetchOptions({ method: requestMethods.GET }), defaultTimeOutMillis);
    
    const apiData: ApiPaginatedResponse<any> = apiResponse.json;
    const apiMeta: any = apiResponse.meta;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('Error getting group folder contents', {
            serviceId: services.GET_GROUP_FOLDER_CONTENTS,
            status: status,
            statusText: statusText,
            body: apiData
        });

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

}