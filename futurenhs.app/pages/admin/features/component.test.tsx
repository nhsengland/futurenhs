import * as React from 'react'
import { cleanup, render, screen } from '@jestMocks/index'
import mockRouter from 'next-router-mock'
import { actions as actionConstants } from '@constants/actions'
import AdminFeaturesPage, { Props } from '@pages/admin/features/index.page'
import { routes } from '@jestMocks/generic-props'

jest.mock('next/router', () => require('next-router-mock'))

describe('Admin features template', () => {
    beforeEach(() => {
        mockRouter.setCurrentUrl('/admin/features')
    })

    const props: Props = {
        id: 'mockId',
        routes: routes,
        contentText: {
            mainHeading: 'Mock main heading',
        },
        actions: [],
        featureFlags: [
            {
                id: 'FlagId',
                name: 'FlagName',
                enabled: true,
            },
        ],
    }

    it('renders correctly', () => {
        render(<AdminFeaturesPage {...props} />)

        expect(screen.getAllByText('Mock main heading').length).toBe(1)
    })

    it('renders feature flags', () => {
        render(<AdminFeaturesPage {...props} />)

        expect(screen.getAllByText('FlagName').length).toBe(1)
    })
})
