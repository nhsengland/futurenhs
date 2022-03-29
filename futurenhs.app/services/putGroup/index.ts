import { setFetchOpts as setFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { cacheNames } from '@constants/caches';
import { clearClientCaches } from '@helpers/util/data';
import { services } from '@constants/services';
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch';
import { ServiceError } from '..';
import { ServiceResponse } from '@appTypes/service';
import { User } from '@appTypes/user';

declare type Options = ({
    groupId: string;
    user: User;
    headers?: any,
    body: FormData;
});

declare type Dependencies = ({
    setFetchOptions: any;
    fetchJSON: any;
});

export const putGroup = async ({
    groupId,
    user,
    headers,
    body
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<null>> => {

    const setFetchOptions = dependencies?.setFetchOptions ?? setFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

    const { id } = user;

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/update`;
    const apiResponse: any = await fetchJSON(apiUrl, setFetchOptions({
        method: requestMethods.PUT,
        headers: headers,
        isMultiPartForm: true,
        body: body
    }), defaultTimeOutMillis);
    
    const apiMeta: any = apiResponse.meta;
    const apiData: any = apiResponse.json;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('Error putting group details', {
            serviceId: services.PUT_GROUP,
            status: status,
            statusText: statusText,
            body: apiData
        });

    }

    await clearClientCaches([cacheNames.NEXT_DATA]);

    return null;

}