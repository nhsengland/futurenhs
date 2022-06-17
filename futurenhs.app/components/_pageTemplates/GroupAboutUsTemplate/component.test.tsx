import * as React from 'react'
import { render, screen } from '@jestMocks/index'

import { GroupAboutUsTemplate } from './index'
import { routes } from '@jestMocks/generic-props'

import { Props } from './interfaces'

describe('Group about us template', () => {
    const props: Props = {
        id: 'mockId',
        routes: routes,
        tabId: 'index',
        actions: [],
        contentText: {
            secondaryHeading: 'Mock secondary heading',
        },
        user: undefined,
        image: null,
        entityText: {
            mainHeading: 'Mock heading',
        },
    }

    it('renders correctly', () => {
        render(<GroupAboutUsTemplate {...props} />)

        expect(screen.getAllByText('Mock secondary heading').length).toBe(1)
    })
})
