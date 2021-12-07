import { render, screen } from '@testing-library/react';

import { Form } from './index';

import { Props } from './interfaces';

const testProps: Props = {
    csrfToken: 'mockToken',
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
        submitButtonText: 'Submit'
    },
    submitAction: jest.fn()
};

describe('Form', () => {

    it('renders fields', () => {

        const props = Object.assign({}, testProps);

        render(<Form {...props} />);

        expect(screen.getByLabelText('mockLabel'));

    });

    it('renders submit button text', () => {

        const props = Object.assign({}, testProps);

        render(<Form {...props} />);

        expect(screen.getByText('Submit'));

    });
    
});
