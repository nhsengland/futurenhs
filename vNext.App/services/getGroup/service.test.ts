import { getGroup } from './index';
import { User } from '@appTypes/user';

const mockUser: User = { 
    id: 'mockUserId',
    fullNameText: '',
    initialsText: '',
    image: {
        source: '',
        altText: ''
    } 
};

let mockSetGetFetchOptions: any;
let mockFetchJSON: any;

describe('getGroup service', () => {

    beforeEach(() => {

        mockSetGetFetchOptions = jest.fn();
        mockFetchJSON = jest.fn();

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

        const response = await getGroup({
            user: mockUser,
            slug: 'mock-slug',
            page: 'home'
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

        const response = await getGroup({
            user: mockUser,
            slug: 'mock-slug',
            page: 'home'
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
                    pageHeader: {
                        nameText: 'mockNameText', 
                        strapLineText: 'Testing unreleased features of the FutureNHS platform',
                        image: {
                            source: '/mockSource',
                            height: 100,
                            width: 100
                        }
                    }
                }
            });

        })

        const response = await getGroup({
            user: mockUser,
            slug: 'mock-slug',
            page: 'home'
        },
        {
            setGetFetchOptions: mockSetGetFetchOptions,
            fetchJSON: mockFetchJSON
        });

        await expect(response.data).toStrictEqual({
            content: {
                mainHeadingHtml: 'mockNameText',
                metaDescriptionText: 'A Future NHS group',
                strapLineText: 'Testing unreleased features of the FutureNHS platform',
                titleText: 'mockNameText'
            },
            image: {
                src: `${process.env.NEXT_PUBLIC_API_BASE_URL}/mockSource`,
                altText: 'TBC',
                height: 100,
                width: 100,
            }
        });

    });
    
});
