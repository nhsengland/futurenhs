export type FetchError = string | object;

export interface FetchOptions {
    method: string;
    credentials: RequestCredentials;
    headers: Headers;
    mode?: 'cors' | 'no-cors' | 'same-origin';
    body?: string;
    contentType?: string;
}