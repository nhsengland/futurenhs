import { withAuth } from './index';
import { ServiceError } from '@services/index';
import { GetAuthService } from '@services/getAuth';

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

describe('withAuth hof', () => {

    it('passes user data into the request context on successful auth', async () => {

        const mockGetServerSideProps: any = jest.fn();
        const mockContext: any = {
            req: {}
        }

        const mockGetAuthService: GetAuthService = () => new Promise((resolve) => {

            resolve({
                data: mockUser
            });
    
        });

        const withOutput = withAuth({
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

        const mockGetAuthService: GetAuthService = () => new Promise((resolve) => {

            throw new ServiceError('No auth', {
                status: 401,
                statusText: 'Denied'
            });
    
        });

        const withOutput = withAuth({
            props: {},
            getServerSideProps: mockGetServerSideProps
        }, {
            getAuthService: mockGetAuthService
        });

        const serverSideProps = await withOutput(mockContext);

        expect(serverSideProps).toHaveProperty('redirect');

    });
    
});
