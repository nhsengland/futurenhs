import { ServiceError } from '@services/index';
import { getUser } from './index';

let mockSetFetchOptions: any;
let mockFetchJSON: any;

describe('getAuth service', () => {

    beforeEach(() => {

        mockSetFetchOptions = jest.fn();
        mockFetchJSON = jest.fn();

    });

    it('creates a cookies header to fetch with', async () => {

        getUser({
            cookies: {
                cookie1: 'value1',
                cookie2: 'value2'
            }
        },
        {
            setFetchOptions: mockSetFetchOptions,
            fetchJSON: mockFetchJSON
        })

        await expect(mockSetFetchOptions).toBeCalledWith({
            method: 'GET',
            headers: {
                Cookie: 'cookie1=value1; cookie2=value2'
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
                    FullName: 'mockUserNameText',
                    UserAvatar: {
                        Source: '/mockSrc',
                        AltText: 'mockAltText'
                    }
                }
            });

        })

        const response = await getUser({
            cookies: {
                cookie1: 'value1',
                cookie2: 'value2'
            }
        },
        {
            setFetchOptions: mockSetFetchOptions,
            fetchJSON: mockFetchJSON
        });

        await expect(response).toStrictEqual({
            data: {
                id: 'mockId',
                text: {
                    userName: 'mockUserNameText'
                },
                image: {
                    source: '/mockSrc',
                    altText: 'mockAltText'
                }
            }
        });

    });
    
});
