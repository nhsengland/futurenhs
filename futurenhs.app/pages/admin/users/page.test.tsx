import * as React from 'react'
import * as nextRouter from 'next/router'
import { render, screen } from '@jestMocks/index'

import AdminUsersTemplate, { getServerSideProps } from './index.page'
import { routes } from '@jestMocks/generic-props'
import { layoutIds } from '@constants/routes'
import { Props } from '@components/_pageTemplates/AdminUsersTemplate/interfaces'

import { mswServer } from '../../../jest-mocks/msw-server'
import { handlers } from '../../../jest-mocks/handlers'
import { actions } from '@constants/actions'

const props: Props = {
    id: 'mockId',
    layoutId: layoutIds.BASE,
    user: { id: 'fake-admin-id', text: { userName: 'Mock User Jest' } },
    contentText: {
        title: 'mockTitle',
        metaDescription: 'mockMetaDescriptionText',
        mainHeading: 'mockMainHeading',
        secondaryHeading: 'Site users',
        noUsers: 'No users to show',
        inviteUser: 'Create a site user',
    },
    usersList: [
        {
            id: 'fake-user-id',
            role: 'Mock member',
            firstName: 'Mock User',
            lastName: 'Mock last name',
            fullName: 'Mock User',
            pronouns: 'User',
            email: 'Mock@test.com',
            requestDate: new Date().toLocaleString(),
            joinDate: new Date().toLocaleString(),
            lastLogInDate: new Date().toLocaleString(),
        },
    ],
    routes: routes,
    actions: [
        actions.GROUPS_EDIT,
        actions.SITE_ADMIN_VIEW,
        actions.SITE_MEMBERS_ADD,
        actions.GROUPS_MEMBERS_EDIT,
        actions.GROUPS_MEMBERS_DELETE,
        actions.GROUPS_MEMBERS_ADD,
        actions.SITE_ADMIN_MEMBERS_DELETE,
        actions.SITE_ADMIN_VIEW,
    ],
}

beforeAll(() => mswServer.listen())
afterEach(() => mswServer.resetHandlers())
afterAll(() => mswServer.close())

describe('admin/users page', () => {
    ;(nextRouter as any).useRouter = jest.fn()
    ;(nextRouter as any).useRouter.mockImplementation(() => ({
        asPath: '/admin/groups',
        query: {},
    }))

    it('renders correctly', () => {
        render(<AdminUsersTemplate {...props} />)

        expect(
            screen.getAllByText(props.usersList[0].firstName).length
        ).not.toEqual(0)
        expect(screen.getAllByText(props.usersList[0].role).length).not.toEqual(
            0
        )
    })

    it('gets required server side props', async () => {
        mswServer.use(handlers.getSiteActions({ actions: props.actions }))

        const serverSideProps = await getServerSideProps({
            req: { cookies: 'fake-cookie-101' },
        } as any)

        expect(serverSideProps).toHaveProperty('props.contentText')
        expect(serverSideProps['notFound']).toBeFalsy()
    })

    it('returns notFound if user does not have admin view permission', async () => {
        const serverSideProps = await getServerSideProps({
            req: { cookies: 'fake-cookie-101' },
        } as any)

        expect(serverSideProps['notFound']).toBe(true)
    })
})
