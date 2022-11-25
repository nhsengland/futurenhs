import * as React from 'react'
import mockRouter from 'next-router-mock'

import { GroupLayout } from './index'
import { routes } from '@jestMocks/generic-props'

import { Props } from './interfaces'
import { render, screen } from '@jestMocks/index'

jest.mock('next/router', () => require('next-router-mock'))

describe('Group Layout', () => {
    beforeEach(() => {
        mockRouter.setCurrentUrl('/groups/groupId')
    })

    const props: Props = {
        tabId: 'index',
        routes: routes,
        entityText: {
            mainHeading: 'Mock heading',
        },
    }

    it('renders correctly', () => {
        render(<GroupLayout {...props} />)

        expect(screen.getAllByText('Mock heading').length).toBe(1)
    })
})
