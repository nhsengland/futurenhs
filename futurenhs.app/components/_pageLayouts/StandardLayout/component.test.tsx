import { render, screen } from '@jestMocks/index'
import { shallow } from 'enzyme'

import { StandardLayout } from './index'

import { Props } from './interfaces'

const testProps: Props = {
    children: undefined,
}

describe('Standard Layout', () => {
    it('renders page content', () => {
        const props = Object.assign({}, testProps)

        render(
            <StandardLayout {...props}>
                <p>Mock page content</p>
            </StandardLayout>
        )

        expect(screen.getByText('Mock page content'))
    })

    it('Includes a custom class name', () => {
        const propsCustomClass = Object.assign({}, testProps, {
            className: 'mock-class',
        })
        const wrapper = shallow(<StandardLayout {...testProps} />)
        const wrapperCustomClass = shallow(
            <StandardLayout {...propsCustomClass} />
        )

        expect(wrapper.find('.mock-class').exists()).toBe(false)
        expect(wrapperCustomClass.find('.mock-class').exists()).toBe(true)
    })
})
