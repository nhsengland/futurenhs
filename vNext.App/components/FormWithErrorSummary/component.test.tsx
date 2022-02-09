import { render, screen } from '@testing-library/react';

import { FormWithErrorSummary } from './index';

import { Props } from './interfaces';

const testProps: Props = {
    csrfToken: 'mockToken',
    errors: {
        error1: 'Mock error'
    },
    fields: [
        {
            name: 'mockFieldName',
            component: 'input',
            text: {
                label: 'mockLabel'
            }
        }
    ],
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

describe('Form', () => {

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
