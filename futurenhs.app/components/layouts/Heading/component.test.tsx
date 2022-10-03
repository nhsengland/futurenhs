import { render, screen } from '@jestMocks/index'
import { shallow } from 'enzyme'

import { Props } from './interfaces'
import { Heading } from './index'

describe('Heading', () => {
    
    const props: Props = {
        children: <p>Test text</p>
    }

    it('Renders correctly', () => {

        render(<Heading {...props} />)

        expect(screen.getAllByText('Test text').length).toBe(1)

    })

    it('Clamps heading levels if provided level is invalid', () => {

        const propsHigh: Props = Object.assign({}, props, { level: 7 })
        const propsLow: Props = Object.assign({}, props, { level: 0 })

        const wrapperHigh = shallow(<Heading {...propsHigh} />)
        const wrapperLow = shallow(<Heading {...propsLow} />)

        expect(wrapperHigh.find('h6').exists()).toBe(true)
        expect(wrapperLow.find('h1').exists()).toBe(true)

    })

});