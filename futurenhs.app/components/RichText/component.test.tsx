import * as React from 'react'
import { shallow } from 'enzyme'

import { RichText } from './index'

import { Props } from './interfaces'

const testProps: Props = {
    bodyHtml: '<p>mock</p>',
}

describe('Rich text', () => {
    it('renders successfully', () => {
        const props = Object.assign({}, testProps)
        const wrapper = shallow(<RichText {...props} />)

        expect(wrapper.find('div').exists()).toBe(true)
    })

    it('adds the provided class to the wrapper element', () => {
        const props = Object.assign({}, testProps, {
            className: 'mock',
        })
        const wrapper = shallow(<RichText {...props} />)

        expect(wrapper.find('div').hasClass('mock')).toBe(true)
    })

    it('different wrapper tags - if the data passed contains HTML', () => {
        const propsSpan = Object.assign({}, testProps, {
            wrapperElementType: 'span',
            bodyHtml: '<strong>span test</strong>',
        })

        const propsPar = Object.assign({}, testProps, {
            wrapperElementType: 'p',
            bodyHtml: '<span>paragraph test</span>',
        })

        const propsH1 = Object.assign({}, testProps, {
            wrapperElementType: 'h1',
            bodyHtml: '<span>h1 test</span>',
        })

        const wrapperSpan = shallow(<RichText {...propsSpan} />)
        const wrapperPar = shallow(<RichText {...propsPar} />)
        const wrapperH1 = shallow(<RichText {...propsH1} />)

        expect(wrapperSpan.find('span').exists()).toBe(true)
        expect(wrapperPar.find('p').exists()).toBe(true)
        expect(wrapperH1.find('h1').exists()).toBe(true)
    })
})
