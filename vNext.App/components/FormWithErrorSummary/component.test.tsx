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
            content: {
                labelText: 'mockLabel'
            }
        }
    ],
    content: {
        errorSummary: {
            bodyHtml: 'An error occurred'
        },
        form: {
            submitButtonText: 'Submit'
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