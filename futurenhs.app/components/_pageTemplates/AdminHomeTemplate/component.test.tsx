import * as React from 'react'
import mockRouter from 'next-router-mock'
import { cleanup, render, screen } from '@jestMocks/index'
import { actions as actionConstants } from '@constants/actions'

import { AdminHomeTemplate } from './index'
import { routes } from '@jestMocks/generic-props'

import { Props } from './interfaces'

jest.mock('next/router', () => require('next-router-mock'))

describe('Admin home template', () => {
    beforeEach(() => {
        mockRouter.setCurrentUrl('/admin/users')
    })

    const props: Props = {
        id: 'mockId',
        routes: routes,
        term: null,
        resultsList: null,
        actions: [
            actionConstants.SITE_ADMIN_MEMBERS_ADD,
            actionConstants.SITE_ADMIN_GROUPS_ADD,
        ],
    }

    it('conditionally renders manage users button', () => {
        render(<AdminHomeTemplate {...props} />)

        expect(screen.getAllByText('Manage users').length).toBe(1)
        expect(screen.getAllByText('Manage groups').length).toBe(1)

        cleanup()

        const propsCopy: Props = Object.assign({}, props, {
            actions: [],
        })

        render(<AdminHomeTemplate {...propsCopy} />)

        expect(screen.queryByText('Manage users')).toBeNull()
        expect(screen.queryByText('Manage groups')).toBeNull()
    })
})
