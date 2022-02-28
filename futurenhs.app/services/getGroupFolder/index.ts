import { setFetchOpts as setFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { ServiceError } from '..';
import { FetchResponse } from '@appTypes/fetch';
import { ApiResponse, ServiceResponse } from '@appTypes/service';
import { Folder } from '@appTypes/file';
import { User } from '@appTypes/user';

declare type Options = ({
    user: User;
    groupId: string;
    folderId: string;
});

declare type Dependencies = ({
    setFetchOptions: any;
    fetchJSON: any;
});

export const getGroupFolder = async ({
    user,
    groupId,
    folderId
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<Folder>> => {

    const serviceResponse: ServiceResponse<Folder> = {
        data: null
    };

    const setFetchOptions = dependencies?.setFetchOptions ?? setFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

    const id: string = user.id;

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/folders/${folderId}`;
    const apiResponse: FetchResponse = await fetchJSON(apiUrl, setFetchOptions({ method: 'GET' }), 30000);
    const apiData: ApiResponse<any> = apiResponse.json;
    const apiMeta: any = apiResponse.meta;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('Error getting group folder', {
            status: status,
            statusText: statusText,
            body: apiData
        });

    }

    const reversedPath: Array<any> = apiData.path?.reverse() ?? [];

    serviceResponse.data = {
        id: apiData.id,
        type: 'folder',
        text: {
            name: apiData.name,
            body: apiData.description
        },
        path: reversedPath.map(({ id, name }) => ({
            element: id,
            text: name
        })) ?? []
    };

    return serviceResponse;

}