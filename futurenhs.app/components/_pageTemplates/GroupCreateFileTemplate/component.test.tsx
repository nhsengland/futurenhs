import { routes } from '@jestMocks/generic-props'
import { render, screen } from '@jestMocks/index'
import forms from '@formConfigs/index'

import { GroupCreateFileTemplate } from './index'

import { Props } from './interfaces'

describe('Group create file template', () => {
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
        contentText: {
            secondaryHeading: 'Mock secondary heading',
        },
        entityText: null,
        image: null,
    }

    it('renders correctly', () => {
        render(<GroupCreateFileTemplate {...props} />)

        expect(screen.getAllByText('Mock secondary heading').length).toBe(1)
    })
})
