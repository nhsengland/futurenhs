import { render, screen } from '@testing-library/react'
import { shallow } from 'enzyme'

import { PageBody } from './index'

import { Props } from './interfaces'

const testProps: Props = {
    children: <p>Mock child content</p>,
}

describe('PageBody', () => {
    it('renders children', () => {
        const props = Object.assign({}, testProps)

        render(<PageBody {...props} />)

        expect(screen.getByText('Mock child content'))
    })

    it('Includes a custom class name', () => {
        const propsCustomClass = Object.assign({}, testProps, {
            className: 'mock-class',
        })
        const wrapper = shallow(<PageBody {...testProps} />)
        const wrapperCustomClass = shallow(<PageBody {...propsCustomClass} />)

        expect(wrapper.find('.mock-class').exists()).toBe(false)
        expect(wrapperCustomClass.find('.mock-class').exists()).toBe(true)
    })
})
