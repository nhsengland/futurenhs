import { render, screen } from '@jestMocks/index'

import { Form } from './index'

import { Props } from './interfaces'

const testProps: Props = {
    csrfToken: 'mockToken',
    formConfig: {
        id: 'mockId',
        steps: [
            {
                fields: [
                    {
                        name: 'mockFieldName',
                        component: 'input',
                        text: {
                            label: 'mockLabel',
                        },
                    },
                ],
            },
        ],
    },
    text: {
        submitButton: 'Submit',
    },
    submitAction: jest.fn(),
}

describe('Form', () => {
    it('renders fields', () => {
        const props = Object.assign({}, testProps)

        render(<Form {...props} />)

        expect(screen.getByLabelText('mockLabel'))
    })

    it('renders submit button text', () => {
        const props = Object.assign({}, testProps)

        render(<Form {...props} />)

        expect(screen.getByText('Submit'))
    })
})
