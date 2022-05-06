import { render, screen } from '@jestMocks/index'

import { Search } from './index'
import { requestMethods } from '@constants/fetch'

import { Props } from './interfaces'

const testProps: Props = {
    method: requestMethods.POST,
    action: '/',
    id: 'mockId',
    text: {
        label: 'mockLabel',
        placeholder: 'mockPlaceholder',
    },
}

describe('Search', () => {
    it('renders', () => {
        const props = Object.assign({}, testProps)

        render(<Search {...props} />)

        expect(screen.getByLabelText('mockLabel'))
        expect(screen.getByPlaceholderText('mockPlaceholder'))
    })
})
