import { setFetchOpts as setFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { services } from '@constants/services';
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch';
import { getCsvStringFromObject } from '@helpers/util/data';
import { ServiceError } from '..';
import { FetchResponse } from '@appTypes/fetch';
import { ApiResponse, ServiceResponse } from '@appTypes/service';
import { User } from '@appTypes/user';
import { CollaboraConnectionParams } from '@appTypes/collabora';

declare type Options = ({
    user: User;
    groupId: string;
    fileId: string;
    cookies: any;
});

declare type Dependencies = ({
    setFetchOptions: any;
    fetchJSON: any;
});

export const getGroupFileView = async ({
    user,
    groupId,
    fileId,
    cookies
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<CollaboraConnectionParams>> => {

    const serviceResponse: ServiceResponse<CollaboraConnectionParams> = {
        data: null
    };

    const setFetchOptions = dependencies?.setFetchOptions ?? setFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;
    const cookieHeader: string = getCsvStringFromObject({
        object: cookies,
        seperator: '; '
    });

    const id: string = user.id;

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/files/${fileId}/view`;
    const apiResponse: FetchResponse = await fetchJSON(apiUrl, setFetchOptions({ 
        method: requestMethods.GET,
        headers: {
            Cookie: cookieHeader
        }
    }), defaultTimeOutMillis);
    
    const apiData: ApiResponse<any> = apiResponse.json;
    const apiMeta: any = apiResponse.meta;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('Error getting group file view', {
            serviceId: services.GET_GROUP_FILE_VIEW,
            status: status,
            statusText: statusText,
            body: apiData
        });

    }

    serviceResponse.data = {
        wopiClientUrl: apiData.WopiClientUrlForFile,
        accessToken: apiData.accessToken
    };
    
    return serviceResponse;

}