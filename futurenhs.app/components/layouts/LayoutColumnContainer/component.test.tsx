import { render, screen } from '@jestMocks/index'

import { LayoutColumnContainer } from './index'

import { Props } from './interfaces'

const testProps: Props = {}

describe('Layout Column Container', () => {
    it('renders child content', () => {
        const props = Object.assign({}, testProps)

        render(
            <LayoutColumnContainer {...props}>
                <p>Mock column content</p>
            </LayoutColumnContainer>
        )

        expect(screen.getByText('Mock column content'))
    })
})
