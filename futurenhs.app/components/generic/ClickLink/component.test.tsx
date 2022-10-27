import { render, screen } from '@jestMocks/index'

import { ActionLink } from './index'

import { Props } from './interfaces'

describe('Action link', () => {
    const props: Props = {
        href: '/',
        text: {
            body: 'Test link',
            ariaLabel: 'Test aria label',
        },
    }

    it('Renders correctly', () => {
        render(<ActionLink {...props} />)

        expect(screen.getAllByText('Test link').length).toBe(1)
    })
})
