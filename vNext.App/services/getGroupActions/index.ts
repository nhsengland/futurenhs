import { actions } from '@constants/actions';
import { setGetFetchOpts as setGetFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { FetchResponse } from '@appTypes/fetch';
import { ApiResponse, ServiceResponse } from '@appTypes/service';
import { User } from '@appTypes/user';

declare type Options = ({
    groupId: string;
    user: User;
});

declare type Dependencies = ({
    setGetFetchOptions: any;
    fetchJSON: any;
});

export type GetGroupActionsService = (options: Options, dependencies?: Dependencies) => Promise<ServiceResponse<Array<actions>>>;

export const getGroupActions = async ({
    groupId,
    user
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<Array<actions>>> => {

    try {

        const setGetFetchOptions = dependencies?.setGetFetchOptions ?? setGetFetchOptionsHelper;
        const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;
        
        const { id } = user;

        const apiUrl: string = `${process.env.NEXT_PUBLIC_API_BASE_URL}/v1/users/${id}/groups/${groupId}/actions`;
        const apiResponse: FetchResponse = await fetchJSON(apiUrl, setGetFetchOptions({}), 30000);
        const apiData: ApiResponse<any> = apiResponse.json;
        const apiMeta: any = apiResponse.meta;

        const { ok, status, statusText } = apiMeta;

        if(!ok){

            return {
                errors: {
                    [status]: statusText
                }
            }

        }

        const data = apiData;

        return {
            data: data,
            errors: {}
        };

    } catch(error){

        const { message } = error;

        return {
            errors: { error: message },
        };

    }

}