import React from 'react'
import { render, screen } from '@jestMocks/index'
import GenericPage, { Props } from '@components/layouts/pages/GenericLayout'
import { routes } from '@jestMocks/generic-props'

const props: Props = {
    id: 'mockId',
    routes: routes,
    user: undefined,
    contentText: {
        title: 'mockTitle',
        metaDescription: 'mockMetaDescriptionText',
        mainHeading: 'mockMainHeading',
    },
}

describe('Generic content template', () => {
    it('renders correctly', () => {
        render(<GenericPage {...props} />)

        expect(screen.getAllByText('mockMainHeading').length).toEqual(1)
    })
})
