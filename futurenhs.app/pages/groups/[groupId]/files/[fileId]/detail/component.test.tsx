import React from 'react'
import mockRouter from 'next-router-mock'
import { render, screen, cleanup } from '@jestMocks/index'
import GroupFileDetailPage, {
    Props,
} from '@pages/groups/[groupId]/files/[fileId]/detail/index.page'
import { routes } from '@jestMocks/generic-props'

jest.mock('next/router', () => require('next-router-mock'))

describe('Group file detail template', () => {
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
            modifiedBy: {
                id: '1',
                text: {
                    userName: 'Mock username',
                },
            },
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
        render(<GroupFileDetailPage {...props} />)

        expect(screen.getAllByText('Mock file name').length).toEqual(2)
    })

    it('conditionally renders breadcrumbs if path is included in props.file', () => {
        render(<GroupFileDetailPage {...props} />)

        expect(screen.getAllByText('Mock breadcrumb').length).toBe(1)

        cleanup()

        const propsCopy: Props = {
            ...props,
            file: {
                ...props.file,
                path: undefined,
            },
        }

        render(<GroupFileDetailPage {...propsCopy} />)

        expect(screen.queryByText('Mock breadcrumb')).toBeNull()
    })

    it('conditionally renders username if createdBy is included in props.file', () => {
        render(<GroupFileDetailPage {...props} />)

        expect(screen.getAllByText('Mock username').length).toBe(2)

        cleanup()

        const propsCopy: Props = {
            ...props,
            file: {
                ...props.file,
                modifiedBy: undefined,
                createdBy: undefined,
            },
        }

        render(<GroupFileDetailPage {...propsCopy} />)

        expect(screen.queryByText('Mock username')).toBeNull()
    })
})
