import React from 'react'
import * as nextRouter from 'next/router'
import { render, screen, cleanup } from '@testing-library/react'

import { groupFolderForm } from '@formConfigs/group-folder'
import { routes } from '@jestMocks/generic-props'
import { GroupCreateUpdateFolderTemplate } from './index'
import { Props } from './interfaces'

describe('Group folder create/update template', () => {
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
        forms: {
            'group-folder': groupFolderForm,
        },
        contentText: null,
        entityText: null,
        image: null,
    }

    it('renders correctly', () => {
        render(<GroupCreateUpdateFolderTemplate {...props} />)

        expect(screen.getAllByText('Save and continue').length).toEqual(1)
    })

    it('conditionally renders folder name when passed folder prop', () => {
        render(<GroupCreateUpdateFolderTemplate {...props} />)

        expect(screen.getAllByText('Mock folder name').length).toEqual(1)

        cleanup()

        const propsCopy = Object.assign({}, props)
        delete propsCopy.folder

        render(<GroupCreateUpdateFolderTemplate {...propsCopy} />)

        expect(screen.queryByText('Mock folder name')).toBeNull()
    })
})
