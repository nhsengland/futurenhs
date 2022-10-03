import React from 'react'
import mockRouter from 'next-router-mock'
import { routes } from '@jestMocks/generic-props'
import { render, screen, cleanup } from '@jestMocks/index'

import GroupFilePreviewPage, {
    Props,
} from '@pages/groups/[groupId]/files/[fileId]/index.page'

jest.mock('next/router', () => require('next-router-mock'))

describe('Group file preview template', () => {
    beforeEach(() => {
        mockRouter.setCurrentUrl('/groups/group/files')
    })

    const props: Props = {
        id: 'mockId',
        routes: routes,
        tabId: 'files',
        fileId: 'mockId',
        file: {
            id: 'mockId',
            type: 'file',
            name: 'Mock file name',
            createdBy: {
                id: '1',
                text: {
                    userName: 'Mock username',
                },
            },
            path: [
                {
                    element: 'p',
                    text: 'Mock breadcrumb',
                },
            ],
        },
        preview: {
            accessToken: '',
            wopiClientUrl: '',
        },
        user: undefined,
        actions: [],
        contentText: null,
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
        render(<GroupFilePreviewPage {...props} />)

        expect(screen.getAllByText('Mock file name').length).toEqual(1)
    })

    it('conditionally renders breadcrumbs if path is included in props.file', () => {
        render(<GroupFilePreviewPage {...props} />)

        expect(screen.getAllByText('Mock breadcrumb').length).toBe(1)

        cleanup()

        const propsCopy: Props = Object.assign({}, props, {
            file: {
                id: 'mockId',
                type: 'file',
                name: 'Mock file name',
            },
        })

        render(<GroupFilePreviewPage {...propsCopy} />)

        expect(screen.queryByText('Mock breadcrumb')).toBeNull()
    })
})
