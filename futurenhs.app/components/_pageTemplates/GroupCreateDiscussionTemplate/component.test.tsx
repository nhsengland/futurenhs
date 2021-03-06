import React from 'react'
import mockRouter from 'next-router-mock'
import { render, screen } from '@jestMocks/index'

import { createDiscussionForm } from '@formConfigs/create-discussion'
import { routes } from '@jestMocks/generic-props'
import { GroupCreateDiscussionTemplate } from './index'
import { Props } from './interfaces'
import forms from '@formConfigs/index'

jest.mock('next/router', () => require('next-router-mock'))

describe('Group create discussion template', () => {
    beforeEach(() => {
        mockRouter.setCurrentUrl('/groups/group/forum/create')
    })

    const props: Props = {
        id: 'mockId',
        routes: routes,
        tabId: 'forum',
        folderId: 'mockId',
        user: undefined,
        actions: [],
        forms: forms,
        contentText: {
            secondaryHeading: 'Mock secondary heading',
        },
        entityText: {
            title: 'Mock title text',
            metaDescription: 'Mock meta description text',
            mainHeading: 'Mock main heading html',
            intro: 'Mock intro html',
            navMenuTitle: 'Mock nav menu title text',
            secondaryHeading: 'Mock secondary heading html',
        },
        image: null,
    }

    it('renders correctly', () => {
        render(<GroupCreateDiscussionTemplate {...props} />)

        expect(screen.getAllByText('Mock secondary heading').length).toEqual(1)
    })
})
