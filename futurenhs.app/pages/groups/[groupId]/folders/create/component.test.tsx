import React from 'react'
import mockRouter from 'next-router-mock'
import { render, screen, cleanup } from '@jestMocks/index'
import { routes } from '@jestMocks/generic-props'
import GroupCreateFolderPage, {
    Props,
} from '@pages/groups/[groupId]/folders/create/index.page'
import forms from '@config/form-configs/index'

jest.mock('next/router', () => require('next-router-mock'))

describe('Group folder create/update template', () => {
    beforeEach(() => {
        mockRouter.setCurrentUrl('/groups/group/files')
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
                name: 'Mock folder name',
            },
        },
        user: undefined,
        actions: [],
        forms: forms,
        contentText: null,
        entityText: null,
        image: null,
    }

    it('renders correctly', () => {
        render(<GroupCreateFolderPage {...props} />)

        expect(screen.getAllByText('Save and continue').length).toEqual(1)
    })

    it('conditionally renders folder name when passed folder prop', () => {
        render(<GroupCreateFolderPage {...props} />)

        expect(screen.getAllByText('Mock folder name').length).toEqual(1)

        cleanup()

        const propsCopy = Object.assign({}, props)
        delete propsCopy.folder

        render(<GroupCreateFolderPage {...propsCopy} />)

        expect(screen.queryByText('Mock folder name')).toBeNull()
    })
})
