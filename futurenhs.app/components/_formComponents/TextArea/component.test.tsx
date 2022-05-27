import { render, screen } from '@jestMocks/index'

import { TextArea } from './index'

import { Props } from './interfaces'

const testProps: Props = {
    input: {
        name: 'mockName',
        value: 'mockValue',
        onChange: jest.fn(),
        onBlur: jest.fn(),
        onFocus: jest.fn(),
    },
    meta: {
        error: '',
        submitError: '',
        touched: false,
    },
    text: {
        label: 'mockLabel',
    },
}

describe('Text area', () => {
    it('renders a label', () => {
        const props = Object.assign({}, testProps)

        render(<TextArea {...props} />)

        expect(screen.getByText('mockLabel'))
    })
})
