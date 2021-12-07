import { withLogOut } from './index';

describe('withLogOut hof', () => {

    it('clears cookies', async () => {

        const mockGetServerSideProps: any = jest.fn();
        const mockContext: any = {
            req: {
                cookies: {
                    cookie1: {},
                    cookie2: {}
                }
            },
            res: {
                cookie: jest.fn()
            }
        }

        const withOutput = withLogOut(mockGetServerSideProps);
        
        await withOutput(mockContext);

        expect(mockContext.res.cookie).toHaveBeenCalledTimes(2);

    });
    
});
