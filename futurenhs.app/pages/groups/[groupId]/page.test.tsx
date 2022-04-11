import * as React from 'react'
import * as nextRouter from 'next/router'
import { render } from '@testing-library/react'

import GroupHomeTemplate, { getServerSideProps } from './index.page'
import { Props } from '@components/_pageTemplates/GroupHomeTemplate/interfaces'
import { routes } from '@jestMocks/generic-props'

import { mswServer } from '../../../jest-mocks/msw-server'
import { handlers } from '../../../jest-mocks/handlers'

beforeAll(() => mswServer.listen())
afterEach(() => mswServer.resetHandlers())
afterAll(() => mswServer.close())

describe.only('group (groups/[groupId]) page', () => {
    ;(nextRouter as any).useRouter = jest.fn()
    ;(nextRouter as any).useRouter.mockImplementation(() => ({
        asPath: '/groups',
        query: {},
    }))

    //Only rendering layout so far -- TO BE COMPLETED

    it.skip('renders correctly', () => {
        const props: Props = {
            id: 'mockId',
            user: { id: 'fake-admin-id', text: { userName: 'Mock User Jest' } },
            contentText: {
                title: 'mockTitle',
                metaDescription: 'mockMetaDescriptionText',
                mainHeading: 'mockMainHeading',
            },
            tabId: 'index',
            image: null,
            entityText: 'Mock group',
            routes: routes,
        }

        render(<GroupHomeTemplate {...props} />)
    })

    it('gets required server side props', async () => {
        const serverSideProps = await getServerSideProps({
            req: { cookies: 'fake-cookie-101' },
        } as any)

        expect(serverSideProps).toHaveProperty('props.entityText')
        expect(serverSideProps).not.toHaveProperty('props.errors')
        expect(serverSideProps['notFound']).toBeFalsy()
    })

    it('returns an error if group not found', async () => {
        mswServer.use(handlers.getGroup({ status: 404 }))

        const serverSideProps = await getServerSideProps({
            req: { cookies: 'fake-cookie-101' },
        } as any)

        expect(serverSideProps['notFound']).toBe(true)
    })
})
