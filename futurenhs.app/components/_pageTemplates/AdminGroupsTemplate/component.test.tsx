import * as React from 'react'
import { cleanup, render, screen } from '@jestMocks/index'
import mockRouter from 'next-router-mock';
import { actions as actionConstants } from '@constants/actions'

import { AdminGroupsTemplate } from './index'
import { routes } from '@jestMocks/generic-props'

import { Props } from './interfaces'

jest.mock('next/router', () => require('next-router-mock'));

describe('Admin Groups Template', () => {

    beforeEach(() => {
        mockRouter.setCurrentUrl('/admin/groups');
    });

    const props: Props = {
        id: 'mockId',
        routes: routes,
        contentText: {
            mainHeading: 'Mock main heading',
            secondaryHeading: 'Mock secondary heading',
            usersHeading: 'Mock users heading',
            noGroups: 'No groups',
            createGroup: 'Create group',
        },
        groupsList: [],
        actions: [],
    }

    it('renders correctly', () => {
        render(<AdminGroupsTemplate {...props} />)

        expect(screen.getAllByText('Mock secondary heading').length).toBe(1)
    })

    it('conditionally renders create user link', () => {
        render(<AdminGroupsTemplate {...props} />)

        expect(screen.queryByText('Create group')).toBeNull()

        cleanup()

        const propsCopy: Props = Object.assign({}, props, {
            actions: [actionConstants.SITE_ADMIN_MEMBERS_ADD],
        })

        render(<AdminGroupsTemplate {...propsCopy} />)

        expect(screen.getAllByText('Create group').length).toBe(1)
    })
})
