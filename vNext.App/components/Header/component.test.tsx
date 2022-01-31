import { render, screen } from '@testing-library/react';

import { Header } from './index';

import { Props } from './interfaces';

const testProps: Props = {
    text: {
        editProfile: 'Edit profile',
        logOut: 'Log out',
        logOutHeading: 'Log out',
        logOutBody: 'Do you want to log out?',
        logOutCancel: 'Cancel',
        logOutConfirm: 'Yes, log out'
    },
    user: {
        id: '12345',
        text: {
            userName: 'Mock Name'
        },
        image: {
            source: '/img.jpg',
            altText: 'Image of Mock Name'
        }
    },
    navMenuList: [],
    shouldRenderSearch: true,
    shouldRenderNavigation: true
};

describe('Header', () => {

    it('Renders a search input', () => {

        const props = Object.assign({}, testProps);

        render(<Header {...props} />);

        expect(screen.getByRole('searchbox'));

    });

    it('Renders a user nav', () => {

        const props = Object.assign({}, testProps);

        render(<Header {...props} />);

        expect(screen.getByRole('navigation'));

    });

    it('Renders content', () => {

        const props = Object.assign({}, testProps);

        render(<Header {...props} />);

        expect(screen.getByText('Log out'));

    });
    
});
