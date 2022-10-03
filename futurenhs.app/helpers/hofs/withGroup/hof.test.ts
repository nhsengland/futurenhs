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

    it('returns group data', async () => {
        expect(
            await withGroup({ 
                req: { 
                    user 
                }, 
                params: { 
                    groupId 
                },
                page: {
                    props: {}
                } 
            } as any, {}, {})
        ).toHaveProperty('props.themeId')
    })

    it('service error thrown', async () => {
        mswServer.use(handlers.getGroup({ status: 500 }))

        const serverSideProps = await withGroup({ 
            req: { 
                user 
            }, 
            params: { 
                groupId 
            },
            page: {
                props: {}
            } 
        } as any, {}, {})

        expect(serverSideProps).toHaveProperty('props.errors')
        expect(serverSideProps['props']['errors'].length).not.toBe(0)
    })

    it('throws an error', async () => {
        try {

            await withGroup({} as any, {}, {})
        } catch (err) {
            expect(err.message.length).not.toBe(0)
        }
    })
})
