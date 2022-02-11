import { setGetFetchOpts as setGetFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { ServiceError } from '..';
import { FetchResponse } from '@appTypes/fetch';
import { ApiResponse, ServiceResponse } from '@appTypes/service';
import { FolderContent } from '@appTypes/file';
import { User } from '@appTypes/user';

declare type Options = ({
    user: User;
    groupId: string;
    fileId: string;
});

declare type Dependencies = ({
    setGetFetchOptions: any;
    fetchJSON: any;
});

export const getGroupFile = async ({
    user,
    groupId,
    fileId
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<FolderContent>> => {

    const serviceResponse: ServiceResponse<FolderContent> = {
        data: null
    };

    const setGetFetchOptions = dependencies?.setGetFetchOptions ?? setGetFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

    const id: string = user.id;

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/files/${fileId}`;
    const apiResponse: FetchResponse = await fetchJSON(apiUrl, setGetFetchOptions({}), 30000);
    const apiData: ApiResponse<any> = apiResponse.json;
    const apiMeta: any = apiResponse.meta;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('Error getting group file', {
            status: status,
            statusText: statusText,
            body: apiData
        });

    }

    const reversedPath: Array<any> = apiData.path?.reverse() ?? [];

    serviceResponse.data = {
        id: apiData.id,
        type: 'file',
        name: apiData.name,
        text: {
            body: apiData.description
        },
        createdBy: {
            id: apiData.firstRegistered?.by?.id,
            text: {
                userName: apiData.firstRegistered?.by?.name
            }
        },
        modifiedBy: {
            id: apiData.lastUpdated?.by?.id,
            text: {
                userName: apiData.lastUpdated?.by?.name
            }
        },
        modified: apiData.lastUpdated?.atUtc,
        path: reversedPath.map(({ id, name }) => ({
            element: id,
            text: name
        })) ?? []
    };

    return serviceResponse;

}