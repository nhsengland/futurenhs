export interface FetchOptions {
    method: string;
    credentials: RequestCredentials;
    headers: Headers;
    mode?: 'cors' | 'no-cors' | 'same-origin';
    body?: string;
}

export interface FetchResponse {
    meta: Response;
    json: any;
}
