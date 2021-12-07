import { render, screen } from '@testing-library/react';

import { HiddenInput } from './index';

import { Props } from './interfaces';

const testProps: Props = {
    input: {
        name: 'mockName',
        value: 'mockValue'
    }
};

describe('Hidden Input', () => {

    it('renders', () => {

        const props = Object.assign({}, testProps);

        render(<HiddenInput {...props} />);

        expect(document.getElementById('mockName'));

    });
    
});
