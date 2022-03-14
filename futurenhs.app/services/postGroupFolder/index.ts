import { setFetchOpts as setFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { services } from '@constants/services';
import { requestMethods } from '@constants/fetch';
import { ServiceError } from '..';
import { ServiceResponse } from '@appTypes/service';
import { User } from '@appTypes/user';

declare type Options = ({
    groupId: string;
    folderId: string;
    user: User;
    csrfToken: string;
    body: FormData;
});

declare type Dependencies = ({
    setFetchOptions: any;
    fetchJSON: any;
});

export const postGroupFolder = async ({
    groupId,
    folderId,
    user,
    csrfToken,
    body
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<null>> => {

    const setFetchOptions = dependencies?.setFetchOptions ?? setFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

    const { id } = user;

    const apiBase: string = typeof window !== 'undefined' ? process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL : process.env.NEXT_PUBLIC_API_BASE_URL;
    const subFolder: string = folderId ? `/${folderId}` : '';
    const apiUrl: string = `${apiBase}/v1/users/${id}/groups/${groupId}/folders${subFolder}`;

    const apiResponse: any = await fetchJSON(apiUrl, setFetchOptions({
        method: requestMethods.POST,
        customHeaders: {
            'csrf-token': csrfToken
        },
        body: {
            Title: body.get('name'),
            Description: body.get('description')
        }
    }), 30000);
    
    const apiMeta: any = apiResponse.meta;
    const apiData: any = apiResponse.json;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('Error posting new group folder', {
            serviceId: services.POST_GROUP_FOLDER,
            status: status,
            statusText: statusText,
            body: apiData
        });

    }

    return null;

}