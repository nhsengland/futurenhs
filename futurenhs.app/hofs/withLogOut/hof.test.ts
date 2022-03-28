import { withLogOut } from './index';
import { authCookie } from '@constants/cookies';

describe('withLogOut hof', () => {

    it('clears cookies', async () => {

        const mockGetServerSideProps: any = jest.fn();
        const mockContext: any = {
            req: {
                cookies: {
                    cookie1: {},
                    [authCookie]: '1234abcd'
                }
            },
            res: {
                cookie: jest.fn()
            }
        }

        const withOutput = withLogOut({
            props: {},
            getServerSideProps: mockGetServerSideProps
        });
        
        await withOutput(mockContext);

        expect(mockContext.res.cookie).toHaveBeenCalledTimes(1);

    });
    
});
