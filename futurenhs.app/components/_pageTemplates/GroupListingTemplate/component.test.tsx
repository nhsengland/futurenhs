import * as React from 'react'
import mockRouter from 'next-router-mock';
import { cleanup, render, screen } from '@jestMocks/index'

import { GroupListingTemplate } from './index'
import { routes } from '@jestMocks/generic-props'
import { Props } from './interfaces'

jest.mock('next/router', () => require('next-router-mock'));

describe('GroupListingTemplate', () => {

    beforeEach(() => {
        mockRouter.setCurrentUrl('/groups');
    });

    const props: Props = {
        id: 'mockPageId',
        routes: routes,
        user: undefined,
        contentText: {
            title: 'Mock title text',
            metaDescription: 'Mock meta description text',
            mainHeading: 'Mock main heading html',
            intro: 'Mock intro html',
            navMenuTitle: 'Mock nav menu title text',
            secondaryHeading: 'Mock secondary heading html',
        },
        isGroupMember: true,
        groupsList: [
            {
                text: {
                    metaDescription: 'Mock meta description text',
                    title: 'Mock title text',
                    mainHeading: 'Mock Group card heading 1',
                },
                groupId: 'mock-group',
                totalDiscussionCount: 3,
                totalMemberCount: 4,
            },
        ],
    }

    it('renders correctly', () => {
        render(<GroupListingTemplate {...props} />)

        expect(screen.getAllByText('Mock main heading html').length).toEqual(1)
    })

    it('renders a group list', () => {
        render(<GroupListingTemplate {...props} />)

        expect(screen.getAllByText('Mock Group card heading 1').length).toEqual(
            1
        )
    })

    it('conditionally renders group image', () => {
        const propsCopy: Props = Object.assign({}, props)
        propsCopy.groupsList[0].image = {
            src: 'https://www.google.com',
            height: 250,
            width: 250,
            altText: 'Mock alt text',
        }

        render(<GroupListingTemplate {...propsCopy} />)

        expect(screen.getAllByAltText('Mock alt text').length).toBe(1)

        cleanup()

        render(<GroupListingTemplate {...props} />)

        expect(screen.queryByAltText('Mock alt text')).toBeNull
    })
})
