import { render, screen } from '@jestMocks/index'

import { WarningCallout } from './index'

import { Props } from './interfaces'

const testProps: Props = {
    headingLevel: 3,
    text: {
        heading: 'Mock heading text',
        body: 'Mock body text'
    },
}

describe('WarningCallout', () => {
    it('renders passed in content', () => {
        const props = Object.assign({}, testProps)

        render(<WarningCallout {...props} />)

        expect(screen.getByText('Mock heading text'))
        expect(screen.getByText('Mock body text'))
    })
})
