import { render, screen } from '@testing-library/react';

import { SkipLinks } from './index';

import { Props } from './interfaces';

const testProps: Props = {
    linkList: [
        {
            id: 'link-1',
            text: 'Link 1'
        },
        {
            id: 'link-2',
            text: 'Link 2'
        }
    ]
};

describe('Skip links', () => {

    it('renders the expected skip links', () => {

        const props = Object.assign({}, testProps);

        render(<SkipLinks {...props} />);

        expect(screen.getByText('Link 1'));
        expect(screen.getByText('Link 2'));

    });
    
});
