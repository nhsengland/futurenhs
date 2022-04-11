import { render, screen } from '@testing-library/react'

import { LayoutColumn } from './index'

import { Props } from './interfaces'

const testProps: Props = {}

describe('Layout Column', () => {
    it('renders child content', () => {
        const props = Object.assign({}, testProps)

        render(
            <LayoutColumn {...props}>
                <p>Mock column content</p>
            </LayoutColumn>
        )

        expect(screen.getByText('Mock column content'))
    })
})
