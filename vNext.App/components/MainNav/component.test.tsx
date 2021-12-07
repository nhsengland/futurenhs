import { render, screen } from '@testing-library/react';

import { MainNav } from './index';

import { Props } from './interfaces';

const testProps: Props = {
    navMenuList: [
        {
            url: '/link-1',
            text: 'Link 1'
        },
        {
            url: '/link-2',
            text: 'Link 2'
        }
    ]
};

describe('Main nav', () => {

    it('renders the expected nav links', () => {

        const props = Object.assign({}, testProps);

        render(<MainNav {...props} />);

        expect(screen.getByTitle('Link 1'));
        expect(screen.getByTitle('Link 2'));

    });
    
});
