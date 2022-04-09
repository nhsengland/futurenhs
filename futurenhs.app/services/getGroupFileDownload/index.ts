import { setFetchOpts as setFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { services } from '@constants/services';
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch';
import { ServiceError } from '..';
import { FetchResponse } from '@appTypes/fetch';
import { ApiResponse, ServiceResponse } from '@appTypes/service';
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
    const apiResponse: FetchResponse = await fetchJSON(apiUrl, setFetchOptions({ method: requestMethods.GET }), defaultTimeOutMillis);
    
    const apiData: ApiResponse<any> = apiResponse.json;
    const apiMeta: any = apiResponse.meta;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('An unexpected error occurred when attempting to get the group file download', {
            serviceId: services.GET_GROUP_FILE,
            status: status,
            statusText: statusText,
            body: apiData
        });

    }

    serviceResponse.data = apiData;

    return serviceResponse;

}