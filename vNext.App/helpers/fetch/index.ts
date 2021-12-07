export interface FetchOptions {
    method: string;
    credentials: RequestCredentials;
    headers: Headers;
    mode?: 'cors' | 'no-cors' | 'same-origin';
    body?: string;
}

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
export const fetchJSON = (url: string, options: FetchOptions, timeOut: number): Promise<{
    meta: Response;
    json: any;
}> => {

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
 * Returns a default options object to use for GET requests
 */
export const setGetFetchOpts = (customHeaders: object): FetchOptions => {

    const fetchOpts: FetchOptions = {
        method: 'GET',
        credentials: 'include',
        headers: getFetchHeadersForJSON(customHeaders)
    };

    return fetchOpts;

};

/**
 * Returns a default options object to use for POST requests
 */
export const setPostFetchOpts = (customHeaders: object = {}, body?: any): FetchOptions => {

    const fetchOpts: FetchOptions = {
        method: 'POST',
        credentials: 'include',
        headers: getFetchHeadersForJSON(customHeaders)
    };

    if(body){

        fetchOpts.body = JSON.stringify(body);

    }

    return fetchOpts;

};

export const timer = (delay: number) => new Promise((resolve) => {

    setTimeout(() => {
        return resolve({});
    }, delay);

});
