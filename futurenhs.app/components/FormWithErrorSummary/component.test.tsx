import { render, screen } from '@testing-library/react';

import { FormWithErrorSummary } from './index';

import { Props } from './interfaces';

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
                            label: 'mockLabel'
                        }
                    }
                ]
            }
        ]
    },
    errors: {
        error1: 'Mock error'
    },
    text: {
        errorSummary: {
            body: 'An error occurred'
        },
        form: {
            submitButton: 'Submit'
        }
    },
    submitAction: jest.fn()
};

describe('Form with error summary', () => {

    it('renders fields', () => {

        const props = Object.assign({}, testProps);

        render(<FormWithErrorSummary {...props} />);

        expect(screen.getByLabelText('mockLabel'));

    });

    it('renders submit button text', () => {

        const props = Object.assign({}, testProps);

        render(<FormWithErrorSummary {...props} />);

        expect(screen.getByText('Submit'));

    });
    
});
