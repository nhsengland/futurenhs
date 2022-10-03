import { shallow } from 'enzyme'

import { Spinner } from './index'

import { Props } from './interfaces'

const testProps: Props = {
    speed: 'medium',
    className: '',
}

describe('Spinner', () => {
    it('renders the expected content', () => {
        const props = Object.assign({}, testProps)

        const wrapper = shallow(<Spinner {...props} />)

        expect(wrapper.find('.c-spinner').exists()).toBe(true);
        expect(wrapper.find('svg').length).toEqual(4);

    })

    it('Includes a custom class name', () => {
        const propsCustomClass = Object.assign({}, testProps, {
            className: 'mock-class',
        })
        const wrapper = shallow(<Spinner {...testProps} />)
        const wrapperCustomClass = shallow(<Spinner {...propsCustomClass} />)

        expect(wrapper.find('.mock-class').exists()).toBe(false)
        expect(wrapperCustomClass.find('.mock-class').exists()).toBe(true)
    })
})
