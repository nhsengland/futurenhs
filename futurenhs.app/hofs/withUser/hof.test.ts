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

describe('withUser hof', () => {
    it('passes user data into the request context on successful auth', async () => {
        const mockGetServerSideProps: any = jest.fn()
        const mockContext: any = {
            req: {},
        }

        const mockGetUserService: GetUserService = () =>
            new Promise((resolve) => {
                resolve({
                    data: mockUser,
                })
            })

        const withOutput = withUser(
            {
                props: {},
                getServerSideProps: mockGetServerSideProps,
            },
            {
                getUserService: mockGetUserService,
            }
        )

        const serverSideProps = await withOutput(mockContext)

        expect(mockContext.req).toHaveProperty('user')
    })

    it('returns redirect instructions on unsuccessful auth', async () => {
        const mockGetServerSideProps: any = jest.fn()
        const mockContext: any = {
            req: {},
        }

        const mockGetUserService: GetUserService = () =>
            new Promise((resolve) => {
                throw new ServiceError('No auth', {
                    serviceId: services.GET_USER,
                    status: 401,
                    statusText: 'Denied',
                })
            })

        const withOutput = withUser(
            {
                props: {},
                getServerSideProps: mockGetServerSideProps,
            },
            {
                getUserService: mockGetUserService,
            }
        )

        const serverSideProps = await withOutput(mockContext)

        expect(serverSideProps).toHaveProperty('redirect')
    })
})
