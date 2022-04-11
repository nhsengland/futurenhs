import { render, screen } from '@testing-library/react'

import { DynamicListContainer } from './index'

import { Props } from './interfaces'

const testProps: Props = {
    containerElementType: 'div',
    children: <p>mock content</p>,
    shouldEnableLoadMore: true,
}

describe('Dynamic List Container', () => {
    it('renders children', () => {
        const props = Object.assign({}, testProps)

        render(<DynamicListContainer {...props} />)

        expect(screen.getByText('mock content'))
    })

    it('throws an error if passed an invalid container element type', () => {
        const propsCopy: Props = Object.assign({}, testProps, {
            containerElementType: 'p',
        })

        expect(() =>
            render(<DynamicListContainer {...propsCopy} />)
        ).toThrowError()
    })
})
