import { render, screen } from '@testing-library/react';

import { Search } from './index';

import { Props } from './interfaces';

const testProps: Props = {
    method: 'POST',
    action: '/',
    id: 'mockId',
    content: {
        labelText: 'mockLabel',
        placeholderText: 'mockPlaceholder'
    }
};

describe('Search', () => {

    it('renders', () => {

        const props = Object.assign({}, testProps);

        render(<Search {...props} />);

        expect(screen.getByLabelText('mockLabel'));
        expect(screen.getByPlaceholderText('mockPlaceholder'));

    });
    
});
