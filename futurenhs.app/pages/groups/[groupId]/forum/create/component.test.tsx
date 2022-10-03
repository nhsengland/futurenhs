import React from 'react'
import mockRouter from 'next-router-mock'
import { render, screen } from '@jestMocks/index'
import { routes } from '@jestMocks/generic-props'
import GroupCreateDiscussionPage, {
    Props,
} from '@pages/groups/[groupId]/forum/create/index.page'
import forms from '@config/form-configs/index'

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
        render(<GroupCreateDiscussionPage {...props} />)

        expect(screen.getAllByText('Mock secondary heading').length).toEqual(1)
    })
})
