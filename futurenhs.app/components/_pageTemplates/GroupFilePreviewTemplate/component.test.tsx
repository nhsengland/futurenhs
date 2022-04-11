import React from 'react'
import * as nextRouter from 'next/router'
import { routes } from '@jestMocks/generic-props'
import { render, screen, cleanup } from '@testing-library/react'

import { GroupFilePreviewTemplate } from './index'
import { Props } from './interfaces'

describe('Group file preview template', () => {
    ;(nextRouter as any).useRouter = jest.fn()
    ;(nextRouter as any).useRouter.mockImplementation(() => ({
        asPath: '/groups/group/files',
        query: {
            groupId: 'group',
        },
    }))

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
        render(<GroupFilePreviewTemplate {...props} />)

        expect(screen.getAllByText('Mock file name').length).toEqual(1)
    })

    it('conditionally renders breadcrumbs if path is included in props.file', () => {
        render(<GroupFilePreviewTemplate {...props} />)

        expect(screen.getAllByText('Mock breadcrumb').length).toBe(1)

        cleanup()

        const propsCopy: Props = Object.assign({}, props, {
            file: {
                id: 'mockId',
                type: 'file',
                name: 'Mock file name',
            },
        })

        render(<GroupFilePreviewTemplate {...propsCopy} />)

        expect(screen.queryByText('Mock breadcrumb')).toBeNull()
    })
})
