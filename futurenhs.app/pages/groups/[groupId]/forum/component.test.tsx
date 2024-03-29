import * as React from 'react'
import mockRouter from 'next-router-mock'
import { cleanup, render, screen } from '@jestMocks/index'
import { actions } from '@constants/actions'

import GroupForumPage, { Props } from '@pages/groups/[groupId]/forum/index.page'
import { routes } from '@jestMocks/generic-props'

jest.mock('next/router', () => require('next-router-mock'))

describe('Group forum template', () => {
    beforeEach(() => {
        mockRouter.setCurrentUrl('/groups/group/forum')
    })

    const props: Props = {
        id: 'mockId',
        routes: routes,
        tabId: 'files',
        user: undefined,
        actions: [actions.GROUPS_DISCUSSIONS_ADD],
        discussionsList: [
            {
                discussionId: 'mockId',
                responseCount: 5,
                viewCount: 20,
                created: '01/01/2022',
                createdBy: {
                    id: 'mockId',
                    text: {
                        userName: 'Mock username',
                    },
                },
                modified: '02/01/2022',
                modifiedBy: {
                    id: 'mockId',
                    text: {
                        userName: 'Mock modifiedBy username',
                    },
                },
                text: {
                    title: 'Mock discussion title',
                    body: 'Mock discussion body',
                },
                isSticky: true,
            },
        ],
        contentText: {
            discussionsHeading: 'Mock content text heading',
            noDiscussions: 'Mock no discussions text',
            createDiscussion: 'Mock create discussion text',
            stickyLabel: 'Mock sticky label',
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
        render(<GroupForumPage {...props} />)

        expect(screen.getAllByText('Mock content text heading').length).toEqual(
            1
        )
    })

    it('conditionally renders discussions', () => {
        render(<GroupForumPage {...props} />)

        expect(screen.getAllByText('Mock discussion title').length).toBe(1)

        cleanup()

        const propsCopy = Object.assign({}, props, {
            discussionsList: [],
        })

        render(<GroupForumPage {...propsCopy} />)

        expect(screen.getAllByText('Mock no discussions text').length).toBe(1)
    })

    it('conditionally renders sticky tag', () => {
        render(<GroupForumPage {...props} />)

        expect(screen.getAllByText('Mock sticky label').length).toBe(1)

        cleanup()

        const propsCopy = JSON.parse(JSON.stringify(props))
        propsCopy.discussionsList[0].isSticky = false

        render(<GroupForumPage {...propsCopy} />)

        expect(screen.queryByText('Mock sticky label')).toBeNull()
    })

    it('conditionally renders last comment by section', () => {
        render(<GroupForumPage {...props} />)

        expect(screen.getAllByText('Mock modifiedBy username').length).toBe(1)

        cleanup()

        const propsCopy = JSON.parse(JSON.stringify(props))
        propsCopy.discussionsList[0].responseCount = 0

        render(<GroupForumPage {...propsCopy} />)

        expect(screen.queryByText('Mock modifiedBy username')).toBeNull()
    })

    it('conditionally renders create discussion button', () => {
        render(<GroupForumPage {...props} />)

        expect(screen.getAllByText('Mock create discussion text').length).toBe(
            1
        )

        cleanup()

        const propsCopy = Object.assign({}, props, {
            actions: [],
        })

        render(<GroupForumPage {...propsCopy} />)

        expect(screen.queryByText('Mock create discussion text')).toBeNull()
    })
})
