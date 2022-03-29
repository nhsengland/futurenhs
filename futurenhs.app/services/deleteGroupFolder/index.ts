import { setFetchOpts as setFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { cacheNames } from '@constants/caches';
import { clearClientCaches } from '@helpers/util/data';
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch';
import { services } from '@constants/services';
import { ServiceError } from '..';
import { ServiceResponse } from '@appTypes/service';
import { User } from '@appTypes/user';

declare type Options = ({
    groupId: string;
    folderId: string;
    user: User;
    csrfToken: string;
});

declare type Dependencies = ({
    setFetchOptions: any;
    fetchJSON: any;
});

export const deleteGroupFolder = async ({
    groupId,
    folderId,
    user,
    csrfToken
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<null>> => {

    const setFetchOptions = dependencies?.setFetchOptions ?? setFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

    const { id } = user;

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/folders/${folderId}`;
    const apiResponse: any = await fetchJSON(apiUrl, setFetchOptions({
        method: requestMethods.DELETE,
        headers: {
            'csrf-token': csrfToken
        }
    }), defaultTimeOutMillis);
    
    const apiMeta: any = apiResponse.meta;
    const apiData: any = apiResponse.json;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('Error deleting group folder', {
            serviceId: services.DELETE_GROUP_FOLDER,
            status: status,
            statusText: statusText,
            body: apiData
        });

    }

    await clearClientCaches([cacheNames.NEXT_DATA]);

    return null;

}