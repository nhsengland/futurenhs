import { render, screen } from '@testing-library/react'

import { Link } from './index'

import { Props } from './interfaces'

const testProps: Props = {
    href: 'mockHref',
    children: 'link',
}

describe('Link', () => {
    it('renders', () => {
        const props = Object.assign({}, testProps)

        render(<Link {...props} />)

        expect(screen.getByText('link'))
    })
})
