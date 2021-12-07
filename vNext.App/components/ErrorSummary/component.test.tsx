import { render, screen } from '@testing-library/react';

import { ErrorSummary } from './index';

import { Props } from './interfaces';

const testProps: Props = {
    content: {
        bodyHtml: '<p>Mock description</p>'
    },
    errors: {
        field1: 'Mock error 1',
        field2: 'Mock error 2'
    },
    className: 'mock-class'
};

describe('Error summary', () => {

    it('renders body html content', () => {

        const props = Object.assign({}, testProps);

        render(<ErrorSummary {...props} />);

        expect(screen.getByText('Mock description'));

    });

    it('renders an error list', () => {

        const props = Object.assign({}, testProps);

        render(<ErrorSummary {...props} />);

        expect(screen.getByText('Mock error 1'));
        expect(screen.getByText('Mock error 2'));

    });
    
});
