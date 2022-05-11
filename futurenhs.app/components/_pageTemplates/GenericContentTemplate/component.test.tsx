import React from 'react'
import { render, screen } from '@jestMocks/index'

import { GenericContentTemplate } from './index'
import { routes } from '@jestMocks/generic-props'

import { Props } from './interfaces'

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
        render(<GenericContentTemplate {...props} />)

        expect(screen.getAllByText('mockMainHeading').length).toEqual(1)
    })
})
