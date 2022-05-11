import * as React from 'react'
import * as nextRouter from 'next/router'
import { render, screen } from '@jestMocks/index'

import AdminHomeTemplate, { getServerSideProps } from './index.page'
import { routes } from '@jestMocks/generic-props'
import { layoutIds } from '@constants/routes'
import { Props } from '@components/_pageTemplates/AdminHomeTemplate/interfaces'

import { mswServer } from '../../jest-mocks/msw-server'
import { actions } from '@constants/actions'

const props: Props = {
    id: 'mockId',
    layoutId: layoutIds.BASE,
    user: { id: 'fake-admin-id', text: { userName: 'Mock User Jest' } },
    contentText: {
        title: 'mockTitle',
        metaDescription: 'mockMetaDescriptionText',
        mainHeading: 'mockMainHeading',
    },
    term: 'cool',
    resultsList: [],
    routes: routes,
    actions: [
        actions.GROUPS_EDIT,
        actions.SITE_ADMIN_VIEW,
        actions.SITE_MEMBERS_ADD,
        actions.GROUPS_MEMBERS_EDIT,
        actions.GROUPS_MEMBERS_DELETE,
        actions.GROUPS_MEMBERS_ADD,
        actions.SITE_ADMIN_MEMBERS_DELETE,
    ],
}

beforeAll(() => mswServer.listen())
afterEach(() => mswServer.resetHandlers())
afterAll(() => mswServer.close())

describe('admin page', () => {
    ;(nextRouter as any).useRouter = jest.fn()
    ;(nextRouter as any).useRouter.mockImplementation(() => ({
        asPath: '/admin/groups',
        query: {},
    }))

    it('renders correctly', () => {
        render(<AdminHomeTemplate {...props} />)

        expect(screen.getAllByText('Manage users').length).toEqual(1)
        expect(screen.getAllByText('Manage groups').length).toEqual(1)
    })

    it('gets required server side props', async () => {
        const serverSideProps = await getServerSideProps({
            req: { cookies: 'ew5tt4t3e' },
        } as any)

        expect(serverSideProps).toHaveProperty('props.contentText')
    })
})
