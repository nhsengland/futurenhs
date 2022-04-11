import { render, screen } from '@testing-library/react'
import { shallow } from 'enzyme'

import { Footer } from './index'

import { Props } from './interfaces'

const testProps: Props = {
    navMenuList: [
        {
            url: '/link-1',
            text: 'Link 1',
        },
        {
            url: '/link-2',
            text: 'Link 2',
        },
    ],
    text: {
        title: 'Footer',
        navMenuAriaLabel: 'Links',
    },
}

describe('Footer', () => {
    it('renders the expected content', () => {
        const props = Object.assign({}, testProps)

        render(<Footer {...props} />)

        expect(screen.getByLabelText('Links'))
    })

    it('renders the expected nav links', () => {
        const props = Object.assign({}, testProps)

        render(<Footer {...props} />)

        expect(screen.getByText('Link 1'))
        expect(screen.getByText('Link 2'))
    })

    it('Includes a custom class name', () => {
        const propsCustomClass = Object.assign({}, testProps, {
            className: 'mock-class',
        })
        const wrapper = shallow(<Footer {...testProps} />)
        const wrapperCustomClass = shallow(<Footer {...propsCustomClass} />)

        expect(wrapper.find('.mock-class').exists()).toBe(false)
        expect(wrapperCustomClass.find('.mock-class').exists()).toBe(true)
    })
})
