import { setFetchOpts as setFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { services } from '@constants/services';
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
    setFetchOptions: any;
    fetchJSON: any;
});

export const getGroupFileDownload = async ({
    user,
    groupId,
    fileId
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<string>> => {

    const serviceResponse: ServiceResponse<string> = {
        data: null
    };

    const setFetchOptions = dependencies?.setFetchOptions ?? setFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

    const id: string = user.id;

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/files/${fileId}/download`;
    const apiResponse: FetchResponse = await fetchJSON(apiUrl, setFetchOptions({ method: 'GET' }), 30000);
    const apiData: ApiResponse<any> = apiResponse.json;
    const apiMeta: any = apiResponse.meta;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('Error getting group file download link', {
            serviceId: services.GET_GROUP_FILE,
            status: status,
            statusText: statusText,
            body: apiData
        });

    }

    serviceResponse.data = apiData;

    return serviceResponse;

}