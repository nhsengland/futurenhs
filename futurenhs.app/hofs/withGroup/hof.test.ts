import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { withGroup } from '.'
import { mswServer } from './../../jest-mocks/msw-server'
import { handlers } from './../../jest-mocks/handlers'

beforeAll(() => mswServer.listen())
afterEach(() => mswServer.resetHandlers())
afterAll(() => mswServer.close())

describe('withGroup hof', () => {
    const groupId: string = 'fake-group-id'
    const user: any = {
        id: 'fake-admin-id',
        FullName: 'Mock User 2',
        UserAvatar: null,
    }
    const props: any = {}

    const getServerSideProps = async (context) => {
        return handleSSRSuccessProps({ props, context })
    }

    it('returns group data', async () => {
        const mockWithGroup = withGroup({ props, getServerSideProps })

        expect(
            await mockWithGroup({ req: { user }, params: { groupId } } as any)
        ).toHaveProperty('props.themeId')
    })

    it('service error thrown', async () => {
        mswServer.use(handlers.getGroup({ status: 500 }))

        const mockWithGroup = withGroup({ props, getServerSideProps })
        const serverSideProps = await mockWithGroup({
            req: { user },
            params: { groupId },
        } as any)

        expect(serverSideProps).toHaveProperty('props.errors')
        expect(serverSideProps['props']['errors'].length).not.toBe(0)
    })

    it('throws an error', async () => {
        try {
            const mockWithGroup = withGroup({ props: null, getServerSideProps })

            await mockWithGroup({ req: { user }, params: { groupId } } as any)
            expect(false).toBe(true)
        } catch (err) {
            expect(err.message.length).not.toBe(0)
        }
    })
})
