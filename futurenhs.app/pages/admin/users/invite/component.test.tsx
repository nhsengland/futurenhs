import * as React from 'react'
import { cleanup, render, screen } from '@jestMocks/index'

import AdminUsersInvitePage, {
    Props,
} from '@pages/admin/users/invite/index.page'
import { routes } from '@jestMocks/generic-props'
import forms from '@config/form-configs/index'

describe('Admin users invite template', () => {
    afterEach(cleanup)

    const props: Props = {
        folderId: 'id',
        routes: routes,
        forms: forms,
        id: 'mockId',
        tabId: null,
        image: null,
        entityText: null,
        contentText: {
            secondaryHeading: 'Mock secondary heading',
        },
    }

    it('renders correctly', () => {
        render(<AdminUsersInvitePage {...props} />)

        expect(screen.getAllByText('Mock secondary heading').length).toBe(1)
    })
})
