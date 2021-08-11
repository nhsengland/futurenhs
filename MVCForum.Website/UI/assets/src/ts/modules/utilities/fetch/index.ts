import { FetchOptions } from '@appTypes/fetch';

/**
 * Returns a default options object to use for JSON fetch requests
 */
export const setFetchOptions = (config: {
    method: 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE'; 
    customHeaders?: any; 
    etag?: string; 
    body?: any; 
    contentType?: string;
}): FetchOptions => {

    const method: 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE' = config.method;
    const customHeaders: any = config.customHeaders ?? {};
    const etag: string = config.etag ?? '*';
    const body: any = config.body;
    const contentType: string = config.contentType ?? 'application/json';

    const headers: Headers = new Headers(Object.assign({}, {
        'Accept': 'application/json',
        'Content-Type': contentType,
        'If-Match': etag
    }, customHeaders));

    if (method !== 'GET') {

        headers.append('X-idempotency-key', Math.random().toString(36).substring(7));

    }

    const fetchOpts: FetchOptions = {
        method: method,
        credentials: 'include',
        headers: headers
    };

    if (body && method !== 'GET') {

        fetchOpts.body = JSON.stringify(body);

    }

    return fetchOpts;

};

/**
 * Generic wrapper for Fetch which will reject on timeOut
 */
export const fetchWithTimeOut = (config: {
    url: string; 
    options: FetchOptions; 
    timeOut: number;
}, dependencies?: {
    fetch: any
}): Promise<any> => {

    const { url, options, timeOut } = config;
    const fetch: any = dependencies?.fetch ?? window.fetch;

    return Promise.race([
        fetch(url, options).then((response: any) => {

            if (!response.ok) {

                throw new Error(`${response.status}: ${response.statusText}`);

            }

            return response;

        }),
        new Promise((_, reject) => setTimeout(() => reject(new Error('The request timed out')), timeOut))
    ]);

};

/**
 * Custom wrapper for Fetch to abstract error handling and JSON parsing
 */
export const fetchData = (config: {
    url: string; 
    options: FetchOptions; 
    timeOut: number;
    dataType?: string;
}): Promise<any> => {

    const { options, dataType } = config;

    return fetchWithTimeOut(config)
        .then((response: any) => {

            /**
             * Parse and return
             */
            return response.text().then((text: string) => {
                
                if(!text.trim()){
                    return null;
                }

                if(dataType === 'html') {
                    
                    return text;
                }

                let parsedResponse: string = '';

                try {

                    parsedResponse = JSON.parse(text);

                } catch (error) {

                    parsedResponse = null;

                }

                return parsedResponse;

            });

        });

};

/**
 * Get an error message string from a passed in error object
 */
export const getErrorMessageString = (error: Error): string => {

    if (!error) {

        return undefined;

    }

    let message: string = error.message || 'Unexpected error';

    return message;

};


export default { 
    fetchData,
    fetchWithTimeOut,
    setFetchOptions, 
    getErrorMessageString
};