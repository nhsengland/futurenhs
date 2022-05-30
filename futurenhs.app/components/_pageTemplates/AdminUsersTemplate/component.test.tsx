import * as React from 'react'
import { cleanup, render, screen } from '@jestMocks/index'
import mockRouter from 'next-router-mock'
import { actions as actionConstants } from '@constants/actions'

import { AdminUsersTemplate } from './index'
import { routes } from '@jestMocks/generic-props'

import { Props } from './interfaces'

jest.mock('next/router', () => require('next-router-mock'))

describe('Admin users template', () => {
    beforeEach(() => {
        mockRouter.setCurrentUrl('/admin/users')
    })

    const props: Props = {
        id: 'mockId',
        routes: routes,
        contentText: {
            mainHeading: 'Mock main heading',
            secondaryHeading: 'Mock secondary heading',
            noUsers: 'No users',
            inviteUser: 'Invite user',
        },
        usersList: [],
        actions: [],
    }

    it('renders correctly', () => {
        render(<AdminUsersTemplate {...props} />)

        expect(screen.getAllByText('Mock secondary heading').length).toBe(1)
    })

    it('renders list of users if there are platform users', () => {
        const propsCopy: Props = Object.assign({}, props, {
            usersList: [
                {
                    id: '123',
                    fullName: 'Test User',
                    role: 'Admin role',
                },
            ],
        })

        render(<AdminUsersTemplate {...propsCopy} />)

        expect(screen.getAllByText('Test User').length).toBe(1)
    })

    it('conditionally renders create user link', () => {
        render(<AdminUsersTemplate {...props} />)

        expect(screen.queryByText('Invite user')).toBeNull()

        cleanup()

        const propsCopy: Props = Object.assign({}, props, {
            actions: [actionConstants.SITE_ADMIN_MEMBERS_ADD],
        })

        render(<AdminUsersTemplate {...propsCopy} />)

        expect(screen.getAllByText('Invite user').length).toBe(1)
    })

    // it('renders users role if no full name', () => {

    //     render(<AdminUsersTemplate {...props}/>);

    //     expect(screen.queryByText('Admin role')).toBeNull();

    //     cleanup();

    //     const propsCopy: Props = Object.assign({}, props, {
    //         usersList: [
    //             {
    //                 id: '123',
    //                 role: 'Admin role'
    //             }
    //         ]
    //     })

    //     render(<AdminUsersTemplate {...propsCopy}/>);

    //     expect(screen.getAllByText('Admin role').length).toBeGreaterThan(0);

    // })
})
