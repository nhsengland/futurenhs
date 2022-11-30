import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { mswServer } from '@jestMocks/msw-server'
import { handlers } from '@jestMocks/handlers'
import fetch from 'jest-fetch-mock'
import { withGroup } from '.'
import { User } from '@appTypes/user'
import { mockUser } from '../withUser/hof.test'

const mockGroup = {
    text: {
        title: 'Mock Group',
        metaDescription: 'A FutureNHS group',
        mainHeading: 'Welcome',
        strapLine: 'To the Future NHS',
    },
    image: null,
    imageId: null,
    themeId: 'ed59e1a4-5cda-4bfb-a32e-dba33e1005be',
    isPublic: true,
}

const mockGroupId: string = 'fake-group-id'
// import { mswServer } from './../../jest-mocks/msw-server'
// import { handlers } from './../../jest-mocks/handlers'

beforeEach(() => {
    fetch.resetMocks()
})

describe('withGroup hof', () => {
    it('returns group data', async () => {
        fetch.mockResponseOnce(JSON.stringify(mockGroup))

        expect(
            await withGroup(
                {
                    req: {
                        user: mockUser,
                    },
                    params: {
                        groupId: mockGroupId,
                    },
                    page: {
                        props: {},
                    },
                } as any,
                {},
                {}
            )
        ).toHaveProperty('props.themeId', mockGroup.themeId)
    })

    it('service error thrown', async () => {
        fetch.mockResponseOnce(
            JSON.stringify(handlers.getGroup({ status: 500 }))
        )

        const serverSideProps = await withGroup(
            {
                req: {
                    user: mockUser,
                },
                params: {
                    groupId: mockGroupId,
                },
                page: {
                    props: {},
                },
            } as any,
            {},
            {}
        )

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
