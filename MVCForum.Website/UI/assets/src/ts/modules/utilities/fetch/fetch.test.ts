import * as fetchHelpers from './index';

const fetchAny = fetch as any;

describe('Fetch helpers', () => {

    it('Should return the message from a passed in error object', () => {

        const expectedResults = 'mock error';
        expect(fetchHelpers.getErrorMessageString(new Error('mock error'))).toBe(expectedResults);

    });

    it('Should return undefined if no error is present', () => {

        expect(fetchHelpers.getErrorMessageString(undefined)).toBe(undefined);

    });

    // fetchWithTimeOut
    it('Should return the correct data from a mocked api response', () => {

        // create mock data and make it available to fetch requests with jest-fetch-mock mock responses (mockResponseOnce)
        const mockData: any = { data: '12345' }
        fetchAny.mockResponseOnce(JSON.stringify( mockData ));

        // call fetch helper as normal, jest-fetch-mock will intercept the request and pass it the mocked response.
        fetchHelpers.fetchWithTimeOut('https://fake.com', {
            method: 'GET',
            credentials: 'omit',
            headers: new Headers()
        }, 9999).then( response => {
            expect(JSON.parse(response.body)).toEqual(mockData);
        });

        // we can also assert that our mock fetch functions have been called, and test the contents of the requests they received.
        expect(fetchAny.mock.calls.length).toEqual(1);
        expect(fetchAny.mock.calls[0][0]).toEqual('https://fake.com');

        fetchAny.resetMocks();
    });

    // fetchData
    it('Should return the correct data from a mocked api response', () => {

        const mockData: any = { data: '12345' }
        fetchAny.mockResponseOnce(JSON.stringify( mockData ));

        fetchHelpers.fetchData('https://fake.com', {
            method: 'GET',
            credentials: 'omit',
            headers: new Headers()
        }, 9999).then( response => {
            expect(JSON.parse(response.body)).toEqual(mockData);
        });

        expect(fetchAny.mock.calls.length).toEqual(1);
        expect(fetchAny.mock.calls[0][0]).toEqual('https://fake.com');

        fetchAny.resetMocks();
    });

    // fetchData with error
    it('Should return an error from a mocked api response', () => {

        fetchAny.mockReject(new Error('Mock Error'));

        fetchHelpers.fetchData('https://fake.com', {
            method: 'GET',
            credentials: 'omit',
            headers: new Headers()
        }, 9999).catch(error => {
            expect(error.message).toBe('Mock Error');
        });

        expect(fetchAny.mock.calls.length).toEqual(1);
        expect(fetchAny.mock.calls[0][0]).toEqual('https://fake.com');

        fetchAny.resetMocks();
    });

});
