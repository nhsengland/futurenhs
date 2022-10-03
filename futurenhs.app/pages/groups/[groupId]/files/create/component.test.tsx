import { routes } from '@jestMocks/generic-props'
import { render, screen } from '@jestMocks/index'
import forms from '@config/form-configs/index'
import GroupCreateFilePage, {
    Props,
} from '@pages/groups/[groupId]/files/create/index.page'

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
        render(<GroupCreateFilePage {...props} />)

        expect(screen.getAllByText('Mock secondary heading').length).toBe(1)
    })
})
