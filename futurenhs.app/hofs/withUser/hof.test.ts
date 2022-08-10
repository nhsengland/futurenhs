import { withUser } from './index'
import { ServiceError } from '@services/index'
import { GetUserService } from '@services/getUser'
import { services } from '@constants/services'

const mockUser = {
    id: 'mockId',
    text: {
        userName: 'A User',
        initials: 'AU',
    },
    image: {
        src: '/mockSrc',
        altText: 'mockAltText',
        height: 100,
        width: 100,
    },
}

const mockGetSiteUserService = () => new Promise((resolve) => resolve({ data: { image: {} } }))
const mockGetSiteActionsService = () => new Promise((resolve) => resolve({ data: [] }))

describe('withUser hof', () => {
    it('passes user data into the request context on successful auth', async () => {

        const mockContext: any = {
            req: {},
            res: {},
            page: {
                props: {}
            }
        } as any

        const mockGetUserService: GetUserService = () =>
            new Promise((resolve) => {
                resolve({
                    data: mockUser,
                })
            })

        await withUser(
            mockContext,
            {},
            {
                getUserService: mockGetUserService,
                getSiteUserService: mockGetSiteUserService,
                getSiteActionsService: mockGetSiteActionsService
            }
        )

        expect(mockContext.req).toHaveProperty('user')
    })

    it('returns redirect instructions on unsuccessful auth', async () => {

        const mockContext: any = {
            req: {},
            res: {},
            page: {
                props: {}
            }
        } as any

        const mockGetUserService: GetUserService = () =>
            new Promise((resolve) => {
                throw new ServiceError('No auth', {
                    serviceId: services.GET_USER,
                    status: 401,
                    statusText: 'Denied',
                })
            })

        const withOutput = await withUser(
            mockContext,
            {},
            {
                getUserService: mockGetUserService,
                getSiteUserService: mockGetSiteUserService,
                getSiteActionsService: mockGetSiteActionsService
            }
        )

        expect(withOutput).toHaveProperty('redirect')
    })
})
