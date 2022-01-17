import { render, screen } from '@testing-library/react';

import { BackLink } from './index';

import { Props } from './interfaces';

const testProps: Props = {
    content: {
        linkText: 'mockContent'    
    },
    href: ''
};

describe('BackLink', () => {

    it('renders link text', () => {

        const props = Object.assign({}, testProps);

        render(<BackLink {...props} />);

        expect(screen.getByText('mockContent'));

    });
    
});
