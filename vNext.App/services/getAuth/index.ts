import { setGetFetchOpts as setGetFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { FetchResponse } from '@appTypes/fetch';
import { ApiResponse, ServiceResponse } from '@appTypes/service';
import { User } from '@appTypes/user';

export type Options = ({
    cookies: Record<string, string>;
});

export type Dependencies = ({
    setGetFetchOptions?: any;
    fetchJSON?: any;
});

export type GetAuthService = (options: Options, dependencies?: Dependencies) => Promise<ServiceResponse<User>>;

export const getAuth: GetAuthService = async ({
    cookies
}, dependencies): Promise<ServiceResponse<User>> => {

    try {

        const setGetFetchOptions = dependencies?.setGetFetchOptions ?? setGetFetchOptionsHelper;
        const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

        let existingCookies: string = '';

        Object.keys(cookies).forEach((name, index) => {

            existingCookies += `${name}=${cookies[name]}${index < Object.keys(cookies).length -1 ? '; ' : ''}`

        });

        const apiUrl: string = process.env.NEXT_PUBLIC_MVC_FORUM_REFRESH_TOKEN_URL;
        const apiResponse: FetchResponse = await fetchJSON(apiUrl, setGetFetchOptions({
            Cookie: existingCookies
        }), 1000);
        const apiData: ApiResponse<any> = apiResponse.json;
        const apiMeta: any = apiResponse.meta;

        const { ok, status, statusText } = apiMeta;

        if(!ok){

            return {
                errors: [{
                    [status]: statusText
                }]
            };

        }

        return {
            data: {
                id: apiData?.Id ?? null,
                text: {
                    userName: apiData?.FullName ?? null
                },
                image: apiData?.UserAvatar ? {
                    source: apiData?.UserAvatar?.Source ?? null,
                    altText: apiData?.UserAvatar?.AltText ?? null
                } : null
            }
        }

    } catch(error){

        const { message } = error;

        return {
            errors: [{ error: message }]
        };

    }

}