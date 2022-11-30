import { withUser } from './index'
import { ServiceError } from '@services/index'
import { services } from '@constants/services'
import { Member } from '@appTypes/member'
import { User } from '@appTypes/user'
import fetch from 'jest-fetch-mock'

export const mockUser: User = {
    id: 'b23fd84e-0dae-44de-a5d4-285288adf40c',
    status: 'Member',
    text: {
        userName: 'John Jones',
    },
    image: null,
    accessToken: '4a8ccca1-e89d-4698-b6d7-dd6a97655e9a',
}
beforeEach(() => {
    fetch.resetMocks()
})

describe('withUser hof', () => {
    it('passes user data into the request context on successful auth', async () => {
        const mockContext: any = {
            params: {
                user: mockUser,
            },
            page: {
                props: {},
            },
        } as any

        await withUser(mockContext, {}, {})

        expect(mockContext).toHaveProperty(
            'params.user.accessToken',
            mockContext.params.user.accessToken
        )
    })

    it('returns redirect instructions on unsuccessful auth', async () => {
        const mockContext: any = {
            req: {},
            res: {},
            page: {
                props: {},
            },
        } as any
        fetch.mockResponseOnce(
            JSON.stringify({
                redirect: 'http://mock-host:5000/auth/signin',
            })
        )
        const withOutput = await withUser(mockContext, {})

        expect(withOutput).toHaveProperty('redirect')
    })
})
