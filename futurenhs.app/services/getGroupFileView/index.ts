import { setFetchOpts as setFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { services } from '@constants/services';
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch';
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
    
    let existingCookies: string = '';

    Object.keys(cookies).forEach((name, index) => {

        existingCookies += `${name}=${cookies[name]}${index < Object.keys(cookies).length -1 ? '; ' : ''}`

    });

    const serviceResponse: ServiceResponse<CollaboraConnectionParams> = {
        data: null
    };

    const setFetchOptions = dependencies?.setFetchOptions ?? setFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

    const id: string = user.id;

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/files/${fileId}/view`;
    const apiResponse: FetchResponse = await fetchJSON(apiUrl, setFetchOptions({ 
        method: requestMethods.GET,
        headers: {
            Cookie: existingCookies
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

    serviceResponse.data = apiData;

    return serviceResponse;

}