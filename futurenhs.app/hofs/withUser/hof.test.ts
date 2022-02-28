import { withUser } from './index';
import { ServiceError } from '@services/index';
import { GetUserService } from '@services/getUser';

const mockUser = {
    id: 'mockId',
    text: {
        userName: 'A User',
        initials: 'AU',
    },
    image: {
        source: '/mockSrc',
        altText: 'mockAltText'
    }
};

describe('withUser hof', () => {

    it('passes user data into the request context on successful auth', async () => {

        const mockGetServerSideProps: any = jest.fn();
        const mockContext: any = {
            req: {}
        }

        const mockGetAuthService: GetUserService = () => new Promise((resolve) => {

            resolve({
                data: mockUser
            });
    
        });

        const withOutput = withUser({
            props: {},
            getServerSideProps: mockGetServerSideProps
        }, {
            getAuthService: mockGetAuthService
        });

        const serverSideProps = await withOutput(mockContext);

        expect(mockContext.req).toHaveProperty('user');

    });

    it('returns redirect instructions on unsuccessful auth', async () => {

        const mockGetServerSideProps: any = jest.fn();
        const mockContext: any = {
            req: {}
        }

        const mockGetAuthService: GetUserService = () => new Promise((resolve) => {

            throw new ServiceError('No auth', {
                status: 401,
                statusText: 'Denied'
            });
    
        });

        const withOutput = withUser({
            props: {},
            getServerSideProps: mockGetServerSideProps
        }, {
            getAuthService: mockGetAuthService
        });

        const serverSideProps = await withOutput(mockContext);

        expect(serverSideProps).toHaveProperty('redirect');

    });
    
});
