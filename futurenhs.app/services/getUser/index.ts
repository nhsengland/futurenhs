import { setFetchOpts as setFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { services } from '@constants/services';
import { ServiceError } from '..';
import { FetchResponse } from '@appTypes/fetch';
import {ApiResponse, ServiceResponse } from '@appTypes/service';
import { User } from '@appTypes/user';

export type Options = ({
    cookies: Record<string, string>;
});

export type Dependencies = ({
    setFetchOptions?: any;
    fetchJSON?: any;
});

export type GetUserService = (options: Options, dependencies?: Dependencies) => Promise<ServiceResponse<User>>;

export const getUser: GetUserService = async ({
    cookies
}, dependencies): Promise<ServiceResponse<User>> => {

    const setFetchOptions = dependencies?.setFetchOptions ?? setFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

    let existingCookies: string = '';

    Object.keys(cookies).forEach((name, index) => {

        existingCookies += `${name}=${cookies[name]}${index < Object.keys(cookies).length -1 ? '; ' : ''}`

    });

    const apiUrl: string = process.env.NEXT_PUBLIC_MVC_FORUM_REFRESH_TOKEN_URL;
    const apiResponse: FetchResponse = await fetchJSON(apiUrl, setFetchOptions({
        method: 'GET',
        customHeaders: {
            Cookie: existingCookies
        }
    }), 1000);

    const apiData: ApiResponse<any> = apiResponse.json;
    const apiMeta: any = apiResponse.meta;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('Error getting user', {
            serviceId: services.GET_USER,
            status: status,
            statusText: statusText,
            body: apiData
        });

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

}