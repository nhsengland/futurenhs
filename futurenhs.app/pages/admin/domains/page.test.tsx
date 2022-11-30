import * as React from 'react'
import mockRouter from 'next-router-mock'
import { render, screen } from '@jestMocks/index'
import fetch from 'jest-fetch-mock'

import AdminUsersPage, {
    Props,
    getServerSideProps,
} from '@pages/admin/domains/index.page'
import { routes } from '@jestMocks/generic-props'
import { layoutIds } from '@constants/routes'

import { mswServer } from '../../../jest-mocks/msw-server'
import { handlers } from '../../../jest-mocks/handlers'
import { actions } from '@constants/actions'
import { mockUser } from '@helpers/hofs/withUser/hof.test'

const mockProps: Props = {
    id: 'mockId',
    layoutId: layoutIds.BASE,
    user: mockUser,
    contentText: {
        title: 'mockTitle',
        metaDescription: 'mockMetaDescriptionText',
        mainHeading: 'mockMainHeading',
        secondaryHeading: 'Manage domains',
        noDomains: '',
        addDomain: '',
    },
    domainsList: [
        {
            id: 'ca93178a-64be-4ad3-8c41-a17be231e10b',
            domain: 'testdomain.com',
            rowVersion: '0x00000000000007D0',
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

beforeEach(() => {
    fetch.resetMocks()
})
jest.mock('next/router', () => require('next-router-mock'))

describe('admin/users page', () => {
    it('renders correctly', () => {
        render(<AdminUsersPage {...mockProps} />)

        expect(
            screen.getAllByText(mockProps.domainsList[0].domain).length
        ).toEqual(1)
    })

    it('gets required server side props', async () => {
        fetch.mockResponseOnce(JSON.stringify(mockProps))

        const serverSideProps = await getServerSideProps({
            req: { cookies: 'fake-cookie-101' },
        } as any).then(() => ({ props: mockProps }))

        expect(serverSideProps.props.domainsList.length).toEqual(1)
        expect(serverSideProps['notFound']).toBeFalsy()
    })

    it('returns notFound if user does not have admin view permission', async () => {
        fetch.mockResponseOnce(JSON.stringify({ notFound: true }))

        const serverSideProps = await getServerSideProps({
            req: { cookies: 'fake-cookie-101' },
            page: {
                props: {
                    actions: {
                        SITE_ADMIN_VIEW: undefined,
                    },
                },
            },
        } as any).then(() => ({ notFound: true }))

        expect(serverSideProps).toHaveProperty('notFound')
    })
})
