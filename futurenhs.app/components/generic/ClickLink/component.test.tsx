import { render, screen } from '@jestMocks/index'
import { ClickLink } from '.'

import { Props } from './interfaces'

describe('Action link', () => {
    const props: Props = {
        onClick: () => null,
        text: {
            body: 'Test link',
            ariaLabel: 'Test aria label',
        },
    }

    it('Renders correctly', () => {
        render(<ClickLink {...props} />)

        expect(screen.getAllByText('Test link').length).toBe(1)
    })
})
