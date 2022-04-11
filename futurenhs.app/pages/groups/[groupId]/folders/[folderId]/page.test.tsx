import * as React from 'react'
import * as nextRouter from 'next/router'
import { fireEvent, render, screen } from '@testing-library/react'

import { routes } from '@jestMocks/generic-props'
import GroupFolderContentsTemplate, { getServerSideProps } from './index.page'
import { Props } from '@components/_pageTemplates/GroupFolderContentsTemplate/interfaces'

import { mswServer } from '../../../../../jest-mocks/msw-server'
import { handlers } from '../../../../../jest-mocks/handlers'
import { actions } from '@constants/actions'

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

beforeAll(() => mswServer.listen())
afterEach(() => mswServer.resetHandlers())
afterAll(() => mswServer.close())

describe('folders/folderId page', () => {
    const push = jest.fn()

    ;(nextRouter as any).useRouter = jest.fn()
    ;(nextRouter as any).useRouter.mockImplementation(() => ({
        asPath: '/folders',
        query: { folderId: 'folderId' },
        push,
    }))

    it('renders correctly', () => {
        render(<GroupFolderContentsTemplate {...props} />)

        expect(screen.getAllByText(props.folder.text.name).length).not.toBe(0)
    })

    it('renders foldersHeading instead of folder header when folderId is missing', () => {
        const propsCopy = Object.assign({}, props, { folderId: null })

        render(<GroupFolderContentsTemplate {...propsCopy} />)

        expect(screen.queryByText(propsCopy.folder.text.name)).toBeNull()
        expect(
            screen.getAllByText(propsCopy.contentText.foldersHeading).length
        ).not.toBe(0)
    })

    it('renders view details link when it is a file', () => {
        const propsCopy = Object.assign({}, props, {
            actions: [
                actions.GROUPS_FOLDERS_EDIT,
                actions.GROUPS_FILES_DELETE,
                actions.GROUPS_FOLDERS_DELETE,
                actions.GROUPS_FILES_EDIT,
            ],
            folderContents: [
                {
                    id: 'file-test',
                    name: 'mock file testing download',
                    type: 'file',
                    downloadLink: 'mock-link',
                    modified: '2020-12-01',
                    modifiedBy: {
                        id: 'mock-editor',
                        text: { userName: 'Mock editor' },
                    },
                    createdBy: {
                        id: 'mock-user',
                        text: { userName: 'Mock user' },
                    },
                },
            ],
        })

        render(<GroupFolderContentsTemplate {...propsCopy} />)

        expect(screen.getAllByText('View details').length).toBe(1)
    })

    it('renders download link when is file', () => {
        const propsCopy = Object.assign({}, props, {
            actions: [
                actions.GROUPS_FOLDERS_EDIT,
                actions.GROUPS_FILES_DELETE,
                actions.GROUPS_FOLDERS_DELETE,
                actions.GROUPS_FILES_EDIT,
            ],
            folderContents: [
                {
                    id: 'file-test',
                    name: 'mock file testing download',
                    type: 'file',
                    downloadLink: 'mock-link',
                    modified: '2020-12-01',
                    modifiedBy: {
                        id: 'mock-editor',
                        text: { userName: 'Mock editor' },
                    },
                    createdBy: {
                        id: 'mock-user',
                        text: { userName: 'Mock user' },
                    },
                },
            ],
        })

        render(<GroupFolderContentsTemplate {...propsCopy} />)

        expect(screen.getAllByText('Download file').length).toBe(1)
    })

    it('does not render view details and download link when not a file', () => {
        const propsCopy = Object.assign({}, props, {
            actions: [
                actions.GROUPS_FOLDERS_EDIT,
                actions.GROUPS_FILES_DELETE,
                actions.GROUPS_FOLDERS_DELETE,
                actions.GROUPS_FILES_EDIT,
            ],
            folderContents: [
                {
                    id: 'folder-test',
                    name: 'mock file testing download',
                    type: 'folder',
                    downloadLink: 'mock-link',
                    modified: '2020-12-01',
                    modifiedBy: {
                        id: 'mock-editor',
                        text: { userName: 'Mock editor' },
                    },
                    createdBy: {
                        id: 'mock-user',
                        text: { userName: 'Mock user' },
                    },
                },
            ],
        })

        render(<GroupFolderContentsTemplate {...propsCopy} />)

        expect(screen.queryAllByText('Download file').length).toBe(0)
        expect(screen.queryAllByText('View details').length).toBe(0)
    })

    it('renders folder body text when available', () => {
        const text = 'Item is a body element'
        const propsCopy = Object.assign({}, props, {
            folder: { text: { body: `<span>${text}</span>` } },
        })

        render(<GroupFolderContentsTemplate {...propsCopy} />)

        expect(screen.getAllByText(text).length).toBe(1)
    })

    it('renders noFolders message', () => {
        const propsCopy = Object.assign({}, props, {
            folderId: null,
            folderContents: [],
        })

        render(<GroupFolderContentsTemplate {...propsCopy} />)

        expect(
            screen.getAllByText(propsCopy.contentText.noFolders).length
        ).toBe(1)
    })

    it('renders createFolder when Add Folders action is present', () => {
        const propsCopy = Object.assign({}, props, {
            actions: [actions.GROUPS_FOLDERS_ADD],
        })

        render(<GroupFolderContentsTemplate {...propsCopy} />)

        expect(
            screen.getAllByText(propsCopy.contentText.createFolder).length
        ).toBe(1)
    })

    it('does not render createFolder when missing Add Folders action', () => {
        const propsCopy = Object.assign({}, props, { actions: [] })

        render(<GroupFolderContentsTemplate {...propsCopy} />)

        expect(
            screen.queryAllByText(propsCopy.contentText.createFolder).length
        ).toBe(0)
    })

    it('renders createFile when Add Files action is present', () => {
        const propsCopy = Object.assign({}, props, {
            actions: [actions.GROUPS_FILES_ADD],
        })

        render(<GroupFolderContentsTemplate {...propsCopy} />)

        expect(
            screen.getAllByText(propsCopy.contentText.createFile).length
        ).toBe(1)
    })

    it('does not render createFile when missing Add Files action', () => {
        const propsCopy = Object.assign({}, props, { actions: [] })

        render(<GroupFolderContentsTemplate {...propsCopy} />)

        expect(
            screen.queryAllByText(propsCopy.contentText.createFile).length
        ).toBe(0)
    })

    it('gets required server side props', async () => {
        const serverSideProps = await getServerSideProps({
            req: { cookies: 'fake-cookie' },
            params: { groupId: 'mock-group-id', folderId: 'mock-folder-id' },
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
            params: { groupId: 'mock-group-id', folderId: 'mock-folder-id' },
        } as any)

        expect(serverSideProps['notFound']).toBe(true)
    })
})
