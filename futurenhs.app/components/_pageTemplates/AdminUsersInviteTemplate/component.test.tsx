import * as React from 'react'
import { cleanup, render, screen } from '@testing-library/react'

import { AdminUsersInviteTemplate } from './index'
import { routes } from '@jestMocks/generic-props'
import forms from '@formConfigs/index'

import { Props } from './interfaces'

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
            secondaryHeading: 'Mock secondary heading'
        }

    }

    it('renders correctly', () => {

        render(<AdminUsersInviteTemplate {...props}/>)

        expect(screen.getAllByText('Mock secondary heading').length).toBe(1);

    })

});