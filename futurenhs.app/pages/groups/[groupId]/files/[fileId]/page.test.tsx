import * as React from 'react'
import mockRouter from 'next-router-mock';
import { render, screen } from '@jestMocks/index'
import { actions } from '@constants/actions';

import { routes } from '@jestMocks/generic-props'
import GroupFilePreviewTemplate, { getServerSideProps } from './index.page'
import { Props } from '@components/_pageTemplates/GroupFilePreviewTemplate/interfaces'

import { mswServer } from '../../../../../jest-mocks/msw-server'
import { handlers } from '../../../../../jest-mocks/handlers'

const props: Props = {
    id: 'mockId',
    user: { id: 'fake-admin-id', text: { userName: 'Mock User Jest' } },
    contentText: {
        title: 'mockTitle',
        metaDescription: 'mockMetaDescriptionText',
        mainHeading: 'mockMainHeading',
    },
    tabId: 'files',
    image: null,
    entityText: 'Mock test file',
    file: {
        id: 'file',
        type: 'file',
        name: 'mock test file',
    },
    preview: {} as any,
    groupId: 'mockGroupId',
    fileId: 'mockFileId',
    routes: routes,
}

jest.mock('next/router', () => require('next-router-mock'));

describe('file page', () => {
    const push = jest.fn()

    beforeAll(() => mswServer.listen())
    afterEach(() => mswServer.resetHandlers())
    afterAll(() => mswServer.close())
    beforeEach(() => {
        mockRouter.setCurrentUrl('/files');
    });

    it('renders correctly', () => {
        render(<GroupFilePreviewTemplate {...props} />)

        expect(screen.getAllByText(props.file.name).length).toEqual(1)
        expect(screen.getAllByText('View details').length).toEqual(1)
    })

    it('gets required server side props', async () => {

        mswServer.use(handlers.getGroupActions({ actions: [actions.GROUPS_VIEW] }))
        
        const serverSideProps = await getServerSideProps({
            req: { cookies: 'fake-cookie-101' },
            params: {
                groupId: 'groupId',
                fileId: 'fileId',
            },
        } as any)

        expect(serverSideProps).toHaveProperty('props.contentText')
        expect(serverSideProps).toHaveProperty('props.entityText')
        expect(serverSideProps['notFound']).toBeFalsy()
    })

    it('returns notFound if user does not have admin view permission', async () => {
        mswServer.use(handlers.getGroup({ status: 404 }))

        const serverSideProps = await getServerSideProps({
            req: { cookies: 'fake-cookie-101' },
        } as any)

        expect(serverSideProps['notFound']).toBe(true)
    })
})
