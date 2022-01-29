import { setGetFetchOpts as setGetFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { FetchResponse } from '@appTypes/fetch';
import { ApiResponse, ServiceResponse } from '@appTypes/service';
import { Group } from '@appTypes/group';

declare type Options = ({
    groupId: string;
});

declare type Dependencies = ({
    setGetFetchOptions: any;
    fetchJSON: any;
});

export type GetGroupService = (options: Options, dependencies?: Dependencies) => Promise<ServiceResponse<Group>>;

export const getGroup = async ({
    groupId
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<Group>> => {

    try {

        const setGetFetchOptions = dependencies?.setGetFetchOptions ?? setGetFetchOptionsHelper;
        const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;
        
        const apiUrl: string = `${process.env.NEXT_PUBLIC_API_BASE_URL}/v1/groups/${groupId}`;
        const apiResponse: FetchResponse = await fetchJSON(apiUrl, setGetFetchOptions({}), 30000);
        const apiData: ApiResponse<any> = apiResponse.json;
        const apiMeta: any = apiResponse.meta;

        const { ok, status, statusText } = apiMeta;

        if(!ok){

            return {
                errors: [{
                    [status]: statusText
                }]
            }

        }

        const data = {
            text: {
                title: apiData.name ?? null, 
                metaDescription: 'A Future NHS group',
                mainHeading: apiData.name ?? null,
                strapLine: apiData?.strapLine ?? null
            },
            image: apiData.image ? {
                src: `${apiData.image?.source}`,
                height: apiData?.image?.height ?? null,
                width: apiData?.image?.width ?? null,
                altText: 'TBC'
            } : null
        };

        return {
            data: data
        };

    } catch(error){

        const { message } = error;

        return {
            errors: [{ error: message }],
        };

    }

}