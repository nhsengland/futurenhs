import { render, screen } from '@testing-library/react'

import { PhaseBanner } from './index'

import { Props } from './interfaces'

const testProps: Props = {
    text: {
        tag: 'beta',
        body: '<p>Mock content</p>',
    },
}

describe('Phase banner', () => {
    it('renders content', () => {
        const props = Object.assign({}, testProps)

        render(<PhaseBanner {...props} />)

        expect(screen.getByText('beta'))
        expect(screen.getByText('Mock content'))
    })
})
