import { requestMethods } from '@constants/fetch';
import { FetchOptions, FetchResponse } from '@appTypes/fetch';

/**
 * Generic wrapper for Fetch which will reject on timeOut
 */
export const fetchWithTimeOut = (url: string, options: FetchOptions, timeOut: number): Promise<any> => {

    return Promise.race([
        fetch(url, options as any).then((response: any) => {

            return response;

        }),
        new Promise((_, reject) => setTimeout(() => reject(new Error('The request timed out')), timeOut))
    ]);

};

/**
 * Custom wrapper for Fetch to abstract error handling and JSON parsing
 */
export const fetchJSON = (url: string, options: FetchOptions, timeOut: number): Promise<FetchResponse> => {

    return fetchWithTimeOut(url, options, timeOut)
        .then((response: Response) => {

            const meta = response.clone();

            return response
                .text()
                .then((text) => {

                    let json: any = '';

                    try {

                        json = JSON.parse(text);

                    } catch(error){

                        json = text;

                    }

                    return {
                        meta: meta,
                        json: json
                    }

                });

        });

};

/**
 * Returns a default options object to use for requests
 */
 export const setFetchOpts = ({
    method,
    headers,
    isMultiPartForm,
    body
}: {
    method: requestMethods,
    headers?: Headers;
    isMultiPartForm?: boolean;
    body?: any;
}): FetchOptions => {

    const headersToUse: Headers = new Headers({
        'Accept': 'application/json'
    });

    if(!isMultiPartForm){

        headersToUse.set('Content-Type', 'application/json');

    }

    if(headers){

        for(const key in headers){

            headersToUse.set(key, headers[key]);

        }

    }

    const fetchOpts: FetchOptions = {
        method: method,
        credentials: 'include',
        headers: headersToUse
    };

    if(body && method === requestMethods.POST || method === requestMethods.PUT || method === requestMethods.PATCH){

        fetchOpts.body = isMultiPartForm ? body : JSON.stringify(body);

    }

    return fetchOpts;

};

/**
 * Generic timer to add timeout functionality to fetch requests 
 */
export const timer = (delay: number) => new Promise((resolve) => {

    setTimeout(() => {
        return resolve({});
    }, delay);

});

/**
 * Returns common headers used in fetch requests by services
 */
export const getStandardServiceHeaders = ({
    csrfToken,
    etag
}: {
    csrfToken: string;
    etag?: string;
}) => {
    
    const headers: Record<string, string> = {};

    if(csrfToken){

        headers['csrf-token'] = csrfToken;

    }

    if(etag){

        headers['If-Match'] = etag;

    }

    return headers;

};
