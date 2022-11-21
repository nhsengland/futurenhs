import * as React from 'react'
import { cleanup, render, screen } from '@jestMocks/index'
import mockRouter from 'next-router-mock'
import { actions as actionConstants } from '@constants/actions'
import AdminDomainsPage, { Props } from '@pages/admin/domains/index.page'
import { routes } from '@jestMocks/generic-props'

jest.mock('next/router', () => require('next-router-mock'))

describe('Admin users template', () => {
    beforeEach(() => {
        mockRouter.setCurrentUrl('/admin/users')
    })

    const props: Props = {
        id: 'mockId',
        routes: routes,
        domainsList: [
            {
                id: '00000000-0000-0000-0000-000000000000',
                domain: 'madetech.com',
                rowVersion: '0x00000000000036F5',
            },
        ],
        contentText: {
            mainHeading: 'Mock main heading',
            secondaryHeading: 'Mock secondary heading',
            noDomains: 'There are currently no accepted domains',
            addDomain: 'Add domain',
        },
    }

    it('renders correctly', () => {
        render(<AdminDomainsPage {...props} />)

        expect(screen.getAllByText('Mock secondary heading').length).toBe(1)
    })

    it('renders list of accepted domains if they have been added', () => {
        render(<AdminDomainsPage {...props} />)

        expect(screen.getAllByText('madetech.com').length).toBe(1)
    })

    it('conditionally renders add domain link', () => {
        render(<AdminDomainsPage {...props} />)

        expect(screen.queryByText('Invite user')).toBeNull()

        cleanup()

        const propsCopy: Props = Object.assign({}, props, {
            actions: [actionConstants.SITE_ADMIN_DOMAINS_ADD],
        })

        render(<AdminDomainsPage {...propsCopy} />)

        expect(screen.getAllByText('Add domain').length).toBe(1)
    })
})
