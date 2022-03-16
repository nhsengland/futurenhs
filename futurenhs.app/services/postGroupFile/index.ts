import { setFetchOpts as setFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { services } from '@constants/services';
import { requestMethods } from '@constants/fetch';
import { ServiceError } from '..';
import { ServiceResponse } from '@appTypes/service';
import { User } from '@appTypes/user';

declare type Options = ({
    groupId: string;
    folderId: string,
    user: User;
    csrfToken: string;
    headers?: any;
    body: FormData;
});

declare type Dependencies = ({
    setFetchOptions: any;
    fetchJSON: any;
});

export const postGroupFile = async ({
    groupId,
    folderId,
    user,
    csrfToken,
    headers = {},
    body
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<null>> => {

    const setFetchOptions = dependencies?.setFetchOptions ?? setFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

    const { id } = user;

    const apiBase: string = process.env.NEXT_PUBLIC_API_BASE_URL;
    const apiUrl: string = `${apiBase}/v1/users/${id}/groups/${groupId}/folders/${folderId}/files`;

    const apiResponse: any = await fetchJSON(apiUrl, setFetchOptions({
        method: requestMethods.POST,
        customHeaders: {
            'csrf-token': csrfToken,
            ...headers
        },
        isMultiPartForm: true,
        body: body
    }), 30000);
    
    const apiMeta: any = apiResponse.meta;
    const apiData: any = apiResponse.json;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('Error posting new group file', {
            serviceId: services.POST_GROUP_FILE,
            status: status,
            statusText: statusText,
            body: apiData
        });

    }

    return null;

}