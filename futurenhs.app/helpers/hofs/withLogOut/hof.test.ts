import { withLogOut } from './index'
import { authCookie } from '@constants/cookies'

describe('withLogOut hof', () => {
    it('clears cookies', async () => {

        const mockContext = {
            req: {
                cookies: {
                    cookie1: {},
                    [authCookie]: '1234abcd',
                },
            },
            res: {
                cookie: jest.fn(),
            },
        } as any
        
        const serverSideProps = await withLogOut(mockContext, {}, {})

        expect(mockContext.res.cookie).toHaveBeenCalledTimes(1)
    })
})
