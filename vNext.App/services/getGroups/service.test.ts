import { getGroups } from './index';
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

describe('getGroups service', () => {

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

        const response = await getGroups({
            user: mockUser,
            filters: {},
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

        const response = await getGroups({
            user: mockUser,
            filters: {},
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
                    data: [{}, {}]
                }
            });

        })

        const response = await getGroups({
            user: mockUser,
            filters: {},
        },
        {
            setGetFetchOptions: mockSetGetFetchOptions,
            fetchJSON: mockFetchJSON
        });

        await expect(response.data.length).toEqual(2);

    });
    
});
