import * as React from 'react'
import mockRouter from 'next-router-mock';
import { render, screen } from '@jestMocks/index'

import { routes } from '@jestMocks/generic-props'
import GroupFolderContentsTemplate, { getServerSideProps } from './index.page'
import { Props } from '@components/_pageTemplates/GroupFolderContentsTemplate/interfaces'

import { mswServer } from '../../../../jest-mocks/msw-server'
import { handlers } from '../../../../jest-mocks/handlers'

const props: Props = {
    id: 'mockId',
    user: { id: 'fake-admin-id', text: { userName: 'Mock User Jest' } },
    contentText: {
        foldersHeading: 'Your content',
        noFolders: 'no content to show',
        createFolder: 'create folder',
        createFile: 'create file',
        updateFolder: 'update folder',
        deleteFolder: 'delete folder',
    },
    tabId: 'files',
    image: null,
    entityText: 'Mock test folders',
    folder: {
        id: 'file',
        type: 'folder',
        text: {
            name: "Mock folder's contents",
        },
    },
    folderId: 'folder-test',
    folderContents: [
        {
            id: 'folder1',
            name: 'mock folder',
            type: 'folder',
            text: { body: 'a simple mock folder description' },
        },
        {
            id: 'file1-2',
            name: 'mock file',
            type: 'file',
            text: {
                body: 'a simple mock file description',
            },
            modified: '2022-4-21',
            modifiedBy: {
                id: 'Mock-tester',
                text: { userName: 'Mock tester' },
            },
            createdBy: {
                id: 'Mock-owner',
                text: { userName: 'Mock Owner' },
            },
            extension: '.xlsx',
        },
        {
            id: 'folder2',
            name: 'mock folder 2',
            type: 'folder',
            downloadLink: 'mock-link',
            createdBy: {
                id: 'mock-user',
                text: { userName: 'Mock user' },
            },
        },
    ],
    routes: routes,
}

describe('folders page', () => {

    beforeAll(() => mswServer.listen())
    afterEach(() => mswServer.resetHandlers())
    afterAll(() => mswServer.close())
    beforeEach(() => {
        mockRouter.setCurrentUrl('/folder/folderId');
    });

    it('renders correctly', () => {
        render(<GroupFolderContentsTemplate {...props} />)

        expect(screen.getAllByText(props.folder.text.name).length).not.toBe(0)
    })

    // TODO: Test only folders are rendered

    it('gets required server side props', async () => {
        const serverSideProps = await getServerSideProps({
            req: { cookies: 'fake-cookie-101' },
        } as any)

        expect(serverSideProps).toHaveProperty('props.contentText')
        expect(serverSideProps).toHaveProperty('props.entityText')
        expect(serverSideProps).not.toHaveProperty('props.errors')
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
