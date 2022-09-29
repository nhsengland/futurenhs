import * as React from 'react'
import mockRouter from 'next-router-mock'
import { render, screen } from '@jestMocks/index'
import AdminGroupsPage, {
    getServerSideProps,
    Props,
} from '@pages/admin/groups/index.page'
import { routes } from '@jestMocks/generic-props'
import { layoutIds } from '@constants/routes'
import { mswServer } from '../../../jest-mocks/msw-server'
import { handlers } from '../../../jest-mocks/handlers'
import { actions } from '@constants/actions'

jest.mock('next/router', () => require('next-router-mock'))

const props: Props = {
    id: 'mockId',
    layoutId: layoutIds.BASE,
    user: { id: 'fake-admin-id', text: { userName: 'Mock User Test' } },
    contentText: {
        title: 'mockTitle',
        metaDescription: 'mockMetaDescriptionText',
        mainHeading: 'mockMainHeading',
        usersHeading: 'adminUsersHeading',
        noGroups: 'No groups to show',
        createGroup: 'Create a group now',
    },
    groupsList: [
        {
            text: {
                mainHeading: 'Mock Group - test group',
            },
            groupId: 'group',
            totalDiscussionCount: 0,
            totalMemberCount: 2,
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

describe('groups page', () => {
    beforeAll(() => mswServer.listen())
    afterAll(() => mswServer.close())
    beforeEach(() => {
        mockRouter.setCurrentUrl('/admin/groups')
    })
    afterEach(() => mswServer.resetHandlers())

    it('renders correctly', () => {
        render(<AdminGroupsPage {...props} />)

        expect(
            screen.getAllByText(props.groupsList[0].text.mainHeading).length
        ).not.toBe(0)
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
