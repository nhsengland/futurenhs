import { render, screen } from '@jestMocks/index'

import { BreadCrumb } from './index'

import { Props } from './interfaces'

const testProps: Props = {
    text: {
        ariaLabel: 'mockContent',
    },
    breadCrumbList: [],
}

describe('BreadCrumb', () => {
    it('renders aria label', () => {
        const props = Object.assign({}, testProps)

        render(<BreadCrumb {...props} />)

        expect(screen.getByLabelText('mockContent'))
    })
})
