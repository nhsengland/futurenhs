import { render, screen } from '@testing-library/react'

import { Card } from './index'

import { Props } from './interfaces'

const testProps: Props = {
    children: <h2>Card</h2>,
}

describe('Card', () => {
    it('renders child content', () => {
        const props = Object.assign({}, testProps)

        render(<Card {...props} />)

        expect(screen.getByText('Card'))
    })
})
