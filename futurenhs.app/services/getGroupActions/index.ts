import { actions } from '@constants/actions';
import { services } from '@constants/services';
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch';
import { ServiceError } from '..';
import { setFetchOpts as setFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { FetchResponse } from '@appTypes/fetch';
import { ApiResponse, ServiceResponse } from '@appTypes/service';
import { User } from '@appTypes/user';

declare type Options = ({
    groupId: string;
    user: User;
});

declare type Dependencies = ({
    setFetchOptions: any;
    fetchJSON: any;
});

export type GetGroupActionsService = (options: Options, dependencies?: Dependencies) => Promise<ServiceResponse<Array<actions>>>;

export const getGroupActions = async ({
    groupId,
    user
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<Array<actions>>> => {

    const setFetchOptions = dependencies?.setFetchOptions ?? setFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

    const { id } = user;

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/actions`;
    const apiResponse: FetchResponse = await fetchJSON(apiUrl, setFetchOptions({ method: requestMethods.GET }), defaultTimeOutMillis);
    
    const apiData: ApiResponse<any> = apiResponse.json;
    const apiMeta: any = apiResponse.meta;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('Error getting group actions', {
            serviceId: services.GET_GROUP_ACTIONS,
            status: status,
            statusText: statusText,
            body: apiData
        });

    }

    const data = apiData;

    return {
        data: data
    };

}