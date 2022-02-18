import { render, screen } from '@testing-library/react';

import { Input } from './index';

import { Props } from './interfaces';

const testProps: Props = {
    input: {
        name: 'mockName',
        value: 'mockValue',
        onChange: jest.fn()
    },
    meta: {
        error: '',
        submitError: '',
        touched: false
    },
    text: {
        label: 'mockLabel'
    }
};

describe('Input', () => {

    it('renders a label', () => {

        const props = Object.assign({}, testProps);

        render(<Input {...props} />);

        expect(screen.getByText('mockLabel'));

    });
    
});
