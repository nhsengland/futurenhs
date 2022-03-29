import { getGroup } from './index';

let mockSetFetchOptions: any;
let mockFetchJSON: any;

describe('getGroup service', () => {

    beforeEach(() => {

        mockSetFetchOptions = jest.fn();
        mockFetchJSON = jest.fn();

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
                    name: 'mockNameText', 
                    strapLine: 'Testing unreleased features of the FutureNHS platform',
                    image: {
                        source: '/mockSource',
                        height: 100,
                        width: 100
                    }
                }
            });

        })

        const response = await getGroup({
            groupId: 'mock-slug',
            user: {
                id: 'mockId',
                text: {
                    userName: 'Mock Name'
                }
            }
        },
        {
            setFetchOptions: mockSetFetchOptions,
            fetchJSON: mockFetchJSON
        });

        await expect(response.data).toStrictEqual({
            text: {
                mainHeading: 'mockNameText',
                metaDescription: 'A Future NHS group',
                strapLine: 'Testing unreleased features of the FutureNHS platform',
                title: 'mockNameText'
            },
            image: {
                src: `/mockSource`,
                altText: 'Group logo',
                height: 100,
                width: 100,
            },
            imageId: undefined,
            themeId: undefined
        });

    });
    
});
