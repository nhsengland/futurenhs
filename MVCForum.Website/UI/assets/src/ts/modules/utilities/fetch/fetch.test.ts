import * as fetchHelpers from './index';

const fetchAny = fetch as any;

describe('Fetch helpers: setFetchOptions', () => {

    it('Should append an idempotency key if the http method is not GET', () => {

        const getHeaders = fetchHelpers.setFetchOptions({
            method: 'GET'
        });
        const postHeaders = fetchHelpers.setFetchOptions({
            method: 'POST'
        });

        expect(getHeaders.headers.has('X-idempotency-key')).toBe(false);
        expect(postHeaders.headers.has('X-idempotency-key')).toBe(true);

    });

    it('Should set a custom content type header if provided', () => {

        const defaultContentTypeHeaders = fetchHelpers.setFetchOptions({
            method: 'GET'
        });
        const customContentTypeHeaders = fetchHelpers.setFetchOptions({
            method: 'GET',
            contentType: 'application/xml'
        });

        expect(defaultContentTypeHeaders.headers.get('Content-Type')).toEqual('application/json');
        expect(customContentTypeHeaders.headers.get('Content-Type')).toEqual('application/xml');

    });

    it('Should set a custom header if provided', () => {

        const noCustomHeaders = fetchHelpers.setFetchOptions({
            method: 'GET'
        });
        const customHeaders = fetchHelpers.setFetchOptions({
            method: 'GET',
            customHeaders: {
                mock: 'header'
            }
        });

        expect(noCustomHeaders.headers.get('mock')).toBeNull();
        expect(customHeaders.headers.get('mock')).toBeDefined();

    });

    it('Should set a custom etag if provided', () => {

        const noCustomEtag = fetchHelpers.setFetchOptions({
            method: 'GET'
        });
        const customEtag = fetchHelpers.setFetchOptions({
            method: 'GET',
            etag: 'mock'
        });

        expect(noCustomEtag.headers.get('If-Match')).toEqual('*');
        expect(customEtag.headers.get('If-Match')).toEqual('mock');

    });

    it('Should add a body if provided and the http method is not GET', () => {

        const getWithBody = fetchHelpers.setFetchOptions({
            method: 'GET',
            body: {
                mock: 'body'
            }
        });
        const postWithoutBody = fetchHelpers.setFetchOptions({
            method: 'POST'
        });
        const postWithBody = fetchHelpers.setFetchOptions({
            method: 'POST',
            body: {
                mock: 'body'
            }
        });

        expect(getWithBody.body).toBeUndefined();
        expect(postWithoutBody.body).toBeUndefined();
        expect(postWithBody.body).toStrictEqual("{\"mock\":\"body\"}");

    });

});

describe('Fetch helpers: fetchWithTimeOut', () => {

    it('Should return the correct data from a mocked api response', () => {

        const mockData: any = { data: '12345' };
        const mockFetch = jest.fn(() => new Promise((resolve: Function) => resolve(JSON.stringify( mockData ))));

        fetchHelpers.fetchWithTimeOut({
            url: 'https://fake.com',
            options: {
                method: 'GET',
                credentials: 'omit',
                headers: new Headers()
            },
            timeOut: 9999
        }, {
            fetch: mockFetch
        }).then( response => {

            expect(JSON.parse(response.body)).toEqual(mockData);
            expect(mockFetch).toBeCalledTimes(1);

        });

    });

    it('Should conditionally reject on timeout', async () => {

        const mockFetch = jest.fn(() => new Promise((resolve: Function) => setTimeout(() => resolve({
            ok: true
        }), 10)));

        await expect(
            
            fetchHelpers.fetchWithTimeOut({
                url: 'https://fake.com',
                options: {
                    method: 'GET',
                    credentials: 'omit',
                    headers: new Headers()
                },
                timeOut: 0
            }, {
                fetch: mockFetch
            })

        ).rejects;

        await expect(
            
            fetchHelpers.fetchWithTimeOut({
                url: 'https://fake.com',
                options: {
                    method: 'GET',
                    credentials: 'omit',
                    headers: new Headers()
                },
                timeOut: 20
            }, {
                fetch: mockFetch
            })

        ).resolves;

    });

});

describe('Fetch helpers: fetchData', () => {

    it('Should return the correct data from a mocked api response', () => {

        const mockData: any = { data: '12345' }
        fetchAny.mockResponseOnce(JSON.stringify( mockData ));

        fetchHelpers.fetchData({
            url: 'https://fake.com',
            options: {
                method: 'GET',
                credentials: 'omit',
                headers: new Headers()
            },
            timeOut: 9999
        }).then( response => {
            expect(JSON.parse(response.body)).toEqual(mockData);
        });

        expect(fetchAny.mock.calls.length).toEqual(1);
        expect(fetchAny.mock.calls[0][0]).toEqual('https://fake.com');

        fetchAny.resetMocks();
    });

    it('Should return null if the response is empty', () => {

        const mockData: any = ' ';
        fetchAny.mockResponseOnce(mockData);

        fetchHelpers.fetchData({
            url: 'https://fake.com',
            options: {
                method: 'GET',
                credentials: 'omit',
                headers: new Headers()
            },
            timeOut: 9999
        }).then( response => {
            expect(JSON.parse(response.body)).toBeNull();
        });

        fetchAny.resetMocks();

    });

    it('Should return null if the json response is invalid', () => {

        const mockData: any = {};
        fetchAny.mockResponseOnce(mockData);

        fetchHelpers.fetchData({
            url: 'https://fake.com',
            options: {
                method: 'GET',
                credentials: 'omit',
                headers: new Headers()
            },
            timeOut: 9999
        }).then( response => {
            expect(JSON.parse(response.body)).toBeNull();
        });

        fetchAny.resetMocks();

    });

    it('Should return html given the appropriate request header', () => {

        const mockData: any = '<html></html>';
        fetchAny.mockResponseOnce(mockData);

        fetchHelpers.fetchData({
            url: 'https://fake.com',
            options: {
                method: 'GET',
                credentials: 'omit',
                contentType: 'text/html',
                headers: new Headers()
            },
            timeOut: 9999
        }).then( response => {
            expect(response.body).toBe('<html></html>');
        });

        fetchAny.resetMocks();
        
    });

    it('Should return an error from a mocked api response', () => {

        fetchAny.mockReject(new Error('Mock Error'));

        fetchHelpers.fetchData({
            url: 'https://fake.com',
            options: {
                method: 'GET',
                credentials: 'omit',
                headers: new Headers()
            },
            timeOut: 9999
        }).catch(error => {
            expect(error.message).toBe('Mock Error');
        });

        expect(fetchAny.mock.calls.length).toEqual(1);
        expect(fetchAny.mock.calls[0][0]).toEqual('https://fake.com');

        fetchAny.resetMocks();

    });

});

describe('Fetch helpers: getErrorMessageString', () => {

    it('Should return the message from a passed in error object', () => {

        expect(fetchHelpers.getErrorMessageString(new Error('mock error'))).toBe('mock error');

    });

    it('Should return a generic message if the passed in error object is missing a message', () => {

        expect(fetchHelpers.getErrorMessageString({
            name: '',
            message: ''
        })).toBe('Unexpected error');

    });

    it('Should return undefined if no error is present', () => {

        expect(fetchHelpers.getErrorMessageString(undefined)).toBe(undefined);

    });

});
