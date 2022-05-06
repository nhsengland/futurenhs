import { render, screen } from '@jestMocks/index'

import { BackLink } from './index'

import { Props } from './interfaces'

const testProps: Props = {
    text: {
        link: 'mockContent',
    },
    href: '',
}

describe('BackLink', () => {
    it('renders link text', () => {
        const props = Object.assign({}, testProps)

        render(<BackLink {...props} />)

        expect(screen.getByText('mockContent'))
    })
})
