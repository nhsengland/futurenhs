import { ServiceError } from '@services/index';
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
