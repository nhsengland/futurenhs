import { shallow } from 'enzyme'

import { Loader } from './index'

import { Props } from './interfaces'

const testProps: Props = {
    className: '',
}

describe('Loader', () => {
    it('Includes a custom class name', () => {
        const propsCustomClass = Object.assign({}, testProps, {
            className: 'mock-class',
        })
        const wrapper = shallow(<Loader {...testProps} />)
        const wrapperCustomClass = shallow(<Loader {...propsCustomClass} />)

        expect(wrapper.find('.mock-class').exists()).toBe(false)
        expect(wrapperCustomClass.find('.mock-class').exists()).toBe(true)
    })
})
