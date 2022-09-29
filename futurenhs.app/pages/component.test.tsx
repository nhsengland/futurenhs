import * as React from 'react'
import { render, screen } from '@jestMocks/index'
import HomePage from '@pages/index.page'
import { routes } from '@jestMocks/generic-props'
import { Props } from '@pages/index.page'

const props: Props = {
    id: 'mockPageId',
    routes: routes,
    user: undefined,
    contentText: {
        title: 'Future NHS Home',
        metaDescription: 'Your Future NHS home page',
        mainHeading: 'Latest discussions',
    },
}

describe('Home template', () => {
    it('renders correctly', () => {
        render(<HomePage {...props} />)

        expect(screen.getByText('Latest discussions'))
    })
})
