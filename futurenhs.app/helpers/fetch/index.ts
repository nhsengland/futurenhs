import { FetchOptions, FetchResponse } from '@appTypes/fetch';

/**
 * Generic wrapper for Fetch which will reject on timeOut
 */
export const fetchWithTimeOut = (url: string, options: FetchOptions, timeOut: number): Promise<any> => {

    return Promise.race([
        fetch(url, options).then((response: any) => {

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
 * Returns a Headers object for standard Fetch JSON requests
 */
export const getFetchHeadersForJSON = (customHeaders: object = {}): Headers => {

    const headers: Headers = new Headers(Object.assign({}, {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
    }, customHeaders));

    return headers;

}

/**
 * Returns a default options object to use for requests
 */
export const setFetchOpts = ({
    method,
    customHeaders,
    body
}: {
    method: 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE',
    customHeaders?: Headers;
    body?: any;
}): FetchOptions => {

    const fetchOpts: FetchOptions = {
        method: method,
        credentials: 'include',
        headers: getFetchHeadersForJSON(customHeaders)
    };

    if(body && method === 'POST' || method === 'PUT' || method === 'PATCH'){

        fetchOpts.body = JSON.stringify(body);

    }

    return fetchOpts;

};

export const timer = (delay: number) => new Promise((resolve) => {

    setTimeout(() => {
        return resolve({});
    }, delay);

});
