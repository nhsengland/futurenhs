import * as React from 'react'
import mockRouter from 'next-router-mock'
import { render, screen } from '@jestMocks/index'
import { routes } from '@jestMocks/generic-props'
import { layoutIds } from '@constants/routes'
import AdminHomePage, {
    Props,
    getServerSideProps,
} from '@pages/admin/index.page'
import { mswServer } from '../../jest-mocks/msw-server'
import { actions } from '@constants/actions'

jest.mock('next/router', () => require('next-router-mock'))

const props: Props = {
    id: 'mockId',
    layoutId: layoutIds.BASE,
    user: {
        id: 'fake-admin-id',
        text: { userName: 'Mock User Jest' },
        status: 'Member',
    },
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

describe('admin page', () => {
    beforeAll(() => mswServer.listen())
    beforeEach(() => {
        mockRouter.setCurrentUrl('/admin/groups')
    })
    afterEach(() => mswServer.resetHandlers())
    afterAll(() => mswServer.close())

    it('renders correctly', () => {
        render(<AdminHomePage {...props} />)

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
