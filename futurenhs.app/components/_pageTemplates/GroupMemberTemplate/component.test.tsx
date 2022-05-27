import * as React from 'react'
import mockRouter from 'next-router-mock'
import { render, screen } from '@jestMocks/index'

import { GroupMemberTemplate } from './index'
import { routes } from '@jestMocks/generic-props'
import forms from '@formConfigs/index'
import { Props } from './interfaces'

jest.mock('next/router', () => require('next-router-mock'))

describe('Group member template', () => {
    beforeEach(() => {
        mockRouter.setCurrentUrl('/groups/group/members/member')
    })

    const props: Props = {
        id: 'mockPageId',
        routes: routes,
        tabId: 'members',
        user: undefined,
        member: {
            firstName: 'Mock first name',
            lastName: 'Mock last name',
            pronouns: 'Mock pronouns',
            email: 'Mock email',
        },
        actions: [],
        forms: forms,
        contentText: {
            secondaryHeading: 'Mock secondary heading html',
        },
        entityText: null,
        image: null,
    }

    it('renders correctly', () => {
        render(<GroupMemberTemplate {...props} />)

        expect(
            screen.getAllByText('Mock secondary heading html').length
        ).toEqual(1)
    })

    it('renders contentText info if provided', () => {
        const propsCopy: Props = Object.assign({}, props, {
            contentText: {
                secondaryHeading: 'Mock secondary heading',
                firstNameLabel: 'Mock first name label',
                lastNameLabel: 'Mock last name label',
                pronounsLabel: 'Mock pronouns label',
                emailLabel: 'Mock email label',
            },
        })

        render(<GroupMemberTemplate {...propsCopy} />)

        expect(screen.getAllByText('Mock first name label').length).toBe(1)
    })
})
