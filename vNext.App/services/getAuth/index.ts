import { setGetFetchOpts as setGetFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { ServiceResponse } from '@appTypes/service';
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
}, dependencies) => {

    try {

        const setGetFetchOptions = dependencies?.setGetFetchOptions ?? setGetFetchOptionsHelper;
        const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

        let existingCookies: string = '';

        Object.keys(cookies).forEach((name, index) => {

            existingCookies += `${name}=${cookies[name]}${index < Object.keys(cookies).length -1 ? '; ' : ''}`

        });

        const { meta, json } = await fetchJSON(process.env.NEXT_PUBLIC_REFRESH_TOKEN_URL, setGetFetchOptions({
            Cookie: existingCookies
        }), 1000)

        const { ok, status, statusText } = meta;

        if(!ok){

            return {
                errors: {
                    [status]: statusText
                }
            };

        }

        return {
            data: {
                id: json?.Id,
                fullNameText: json?.FullNameText,
                initialsText: json?.InitialsText,
                image: {
                    source: json?.UserAvatar?.Source,
                    altText: json?.UserAvatar?.AltText
                }
            }
        }

    } catch(error){

        const { message } = error;

        return {
            errors: { error: message }
        };

    }

}