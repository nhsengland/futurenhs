import { getAuth } from './index';

let mockSetGetFetchOptions: any;
let mockFetchJSON: any;

describe('getAuth service', () => {

    beforeEach(() => {

        mockSetGetFetchOptions = jest.fn();
        mockFetchJSON = jest.fn();

    });

    it('creates a cookies header to fetch with', async () => {

        getAuth({
            cookies: {
                cookie1: 'value1',
                cookie2: 'value2'
            }
        },
        {
            setGetFetchOptions: mockSetGetFetchOptions,
            fetchJSON: mockFetchJSON
        })

        await expect(mockSetGetFetchOptions).toBeCalledWith({
            Cookie: 'cookie1=value1; cookie2=value2'
        });

    });

    it('returns an error if the fetch failed', async () => {

        mockFetchJSON = () => new Promise((resolve) => {

            const response: Partial<Response> = {
                ok: false,
                status: 500,
                statusText: 'Something went wrong'
            };

            resolve({
                meta: response,
                json: {}
            });

        })

        const response = await getAuth({
            cookies: {
                cookie1: 'value1',
                cookie2: 'value2'
            }
        },
        {
            setGetFetchOptions: mockSetGetFetchOptions,
            fetchJSON: mockFetchJSON
        });

        await expect(response).toStrictEqual({
            errors: {
                [500]: 'Something went wrong'
            }
        });

    });

    it('returns an internal error if it occurs', async () => {

        mockFetchJSON = () => new Promise((resolve) => {

            resolve({
                json: {}
            });

        })

        const response = await getAuth({
            cookies: {
                cookie1: 'value1',
                cookie2: 'value2'
            }
        },
        {
            setGetFetchOptions: mockSetGetFetchOptions,
            fetchJSON: mockFetchJSON
        });

        await expect(response).toStrictEqual({
            errors: {
                error: "Cannot read property 'ok' of undefined"
            }
        });

    });

    it('returns a data object on success', async () => {

        mockFetchJSON = () => new Promise((resolve) => {

            const response: Partial<Response> = {
                ok: true,
                status: 200,
                statusText: 'Success'
            };

            resolve({
                meta: response,
                json: {
                    Id: 'mockId',
                    FullName: 'mockFullNameText',
                    Initials: 'mockInitialsText',
                    UserAvatar: {
                        Source: '/mockSrc',
                        AltText: 'mockAltText'
                    }
                }
            });

        })

        const response = await getAuth({
            cookies: {
                cookie1: 'value1',
                cookie2: 'value2'
            }
        },
        {
            setGetFetchOptions: mockSetGetFetchOptions,
            fetchJSON: mockFetchJSON
        });

        await expect(response).toStrictEqual({
            data: {
                id: 'mockId',
                fullNameText: 'mockFullNameText',
                initialsText: 'mockInitialsText',
                image: {
                    source: '/mockSrc',
                    altText: 'mockAltText'
                }
            }
        });

    });
    
});
