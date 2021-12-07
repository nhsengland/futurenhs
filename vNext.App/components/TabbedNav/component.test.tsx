import { render, screen } from '@testing-library/react';

import { TabbedNav } from './index';

import { Props } from './interfaces';

const testProps: Props = {
    content: {
        ariaLabelText: 'mockLabel'    
    },
    navMenuList: []
};

describe('TabbedNav', () => {

    it('renders description Html', () => {

        const props = Object.assign({}, testProps);

        render(<TabbedNav {...props} />);

        expect(screen.getByLabelText('mockLabel'));

    });
    
});
