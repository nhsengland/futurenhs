import { render } from '@testing-library/react'

import { Hidden } from './index'

import { Props } from './interfaces'

const testProps: Props = {
    id: 'mockId',
    input: {
        name: 'mockName',
        value: 'mockValue',
    },
}

describe('Hidden Input', () => {
    it('renders', () => {
        const props = Object.assign({}, testProps)

        render(<Hidden {...props} />)

        expect(document.getElementById('mockName'))
    })
})
