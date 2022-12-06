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
            secondaryHeading: 'Mock secondary heading',
        },
        actions: [],
    }

    it('renders correctly', () => {
        render(<AdminFeaturesPage {...props} />)

        expect(screen.getAllByText('Mock main heading').length).toBe(1)

        expect(screen.getAllByText('Mock secondary heading').length).toBe(1)
    })
})
