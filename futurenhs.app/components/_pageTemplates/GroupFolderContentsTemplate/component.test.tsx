import React from 'react'
import mockRouter from 'next-router-mock'
import { render, screen, cleanup } from '@jestMocks/index'
import { actions as userActions } from '@constants/actions'

import { GroupFolderContentsTemplate } from './index'
import { routes } from '@jestMocks/generic-props'
import { Props } from './interfaces'

jest.mock('next/router', () => require('next-router-mock'))

describe('Group folders template', () => {
    beforeEach(() => {
        mockRouter.setCurrentUrl('/groups/group/folder')
    })

    const props: Props = {
        id: 'mockId',
        routes: routes,
        tabId: 'files',
        folderId: 'mockId',
        folder: {
            id: 'mockId',
            type: 'folder',
            text: {
                name: 'Mock folder',
                body: 'Mock folder body text',
            },
            path: [
                {
                    element: 'p',
                    text: 'Mock breadcrumb',
                },
            ],
        },
        folderContents: [
            {
                id: 'mockId',
                name: 'Mock folder content name',
                type: 'file',
            },
        ],
        user: undefined,
        actions: [
            userActions.GROUPS_FOLDERS_EDIT,
            userActions.GROUPS_FOLDERS_DELETE,
            userActions.GROUPS_FOLDERS_ADD,
            userActions.GROUPS_FILES_ADD,
        ],
        contentText: {
            foldersHeading: 'Mock folder heading',
            noFolders: 'No folders',
            createFolder: 'Create folder',
            updateFolder: 'Update folder',
            deleteFolder: 'Delete folder',
            createFile: 'Upload file',
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
        render(<GroupFolderContentsTemplate {...props} />)

        expect(screen.getAllByText('Mock folder').length).toEqual(1)
    })

    it('conditionally renders breadcrumbs if path is included in props.folder', () => {
        render(<GroupFolderContentsTemplate {...props} />)

        expect(screen.getAllByText('Mock breadcrumb').length).toBe(1)

        cleanup()

        const propsCopy: Props = Object.assign({}, props, {
            folder: {
                id: 'mockId',
                type: 'file',
                text: {
                    name: 'Mock folder',
                },
            },
        })

        render(<GroupFolderContentsTemplate {...propsCopy} />)

        expect(screen.queryByText('Mock breadcrumb')).toBeNull()
    })

    it('conditionally renders delete/edit buttons', () => {
        render(<GroupFolderContentsTemplate {...props} />)

        expect(screen.getAllByText('Update folder').length).toBe(1)
        // expect(screen.getAllByText('Delete folder').length).toBe(0);

        cleanup()

        const propsCopy: Props = Object.assign({}, props, {
            actions: [],
        })

        render(<GroupFolderContentsTemplate {...propsCopy} />)

        expect(screen.queryByText('Update folder')).toBeNull()
        expect(screen.queryByText('Delete folder')).toBeNull()
    })

    it('conditionally renders folder name', () => {
        render(<GroupFolderContentsTemplate {...props} />)

        expect(screen.getAllByText('Mock folder').length).toBe(1)

        cleanup()

        const propsCopy: Props = Object.assign({}, props, {
            folderId: null,
        })

        render(<GroupFolderContentsTemplate {...propsCopy} />)

        expect(screen.getAllByText('Mock folder heading').length).toBe(1)
    })

    it('conditionally renders folder body text', () => {
        render(<GroupFolderContentsTemplate {...props} />)

        expect(screen.getAllByText('Mock folder body text').length).toBe(1)

        cleanup()

        const propsCopy: Props = Object.assign({}, props, {
            folder: {
                id: 'mockId',
                type: 'folder',
                text: {
                    name: 'Mock folder',
                },
            },
        })

        render(<GroupFolderContentsTemplate {...propsCopy} />)

        expect(screen.queryByText('Mock folder body text')).toBeNull()
    })

    it('conditionally renders folder contents', () => {
        render(<GroupFolderContentsTemplate {...props} />)

        expect(screen.getAllByText('Mock folder content name').length).toBe(1)

        cleanup()

        const propsCopy: Props = Object.assign({}, props, {
            folderContents: null,
            folderId: null,
        })

        render(<GroupFolderContentsTemplate {...propsCopy} />)

        expect(screen.getAllByText('No folders').length).toBe(1)
    })

    it('conditionally renders create folder button', () => {
        render(<GroupFolderContentsTemplate {...props} />)

        expect(screen.getAllByText('Create folder').length).toBe(1)

        cleanup()

        const propsCopy: Props = Object.assign({}, props, {
            actions: [
                userActions.GROUPS_FOLDERS_EDIT,
                userActions.GROUPS_FOLDERS_DELETE,
                userActions.GROUPS_FILES_ADD,
            ],
        })

        render(<GroupFolderContentsTemplate {...propsCopy} />)

        expect(screen.queryByText('Create folder')).toBeNull()
    })

    it('conditionally renders create folder button', () => {
        render(<GroupFolderContentsTemplate {...props} />)

        expect(screen.getAllByText('Upload file').length).toBe(1)

        cleanup()

        const propsCopy: Props = Object.assign({}, props, {
            actions: [
                userActions.GROUPS_FOLDERS_EDIT,
                userActions.GROUPS_FOLDERS_DELETE,
                userActions.GROUPS_FOLDERS_ADD,
            ],
        })

        render(<GroupFolderContentsTemplate {...propsCopy} />)

        expect(screen.queryByText('Upload file')).toBeNull()
    })
})
