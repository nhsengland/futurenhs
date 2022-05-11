import { render, screen } from '@jestMocks/index'

import { TabbedNav } from './index'

import { Props } from './interfaces'

const testProps: Props = {
    text: {
        ariaLabel: 'mockLabel',
    },
    navMenuList: [],
}

describe('TabbedNav', () => {
    it('renders description Html', () => {
        const props = Object.assign({}, testProps)

        render(<TabbedNav {...props} />)

        expect(screen.getByLabelText('mockLabel'))
    })
})
