import { getGroup } from './index';
import { User } from '@appTypes/user';

let mockSetGetFetchOptions: any;
let mockFetchJSON: any;

describe('getGroup service', () => {

    beforeEach(() => {

        mockSetGetFetchOptions = jest.fn();
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
            groupId: 'mock-slug'
        },
        {
            setGetFetchOptions: mockSetGetFetchOptions,
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
                altText: 'TBC',
                height: 100,
                width: 100,
            }
        });

    });
    
});
