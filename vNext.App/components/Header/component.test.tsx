import { render, screen } from '@testing-library/react';

import { Header } from './index';

import { Props } from './interfaces';

const testProps: Props = {
    content: {
        editProfileText: 'Edit profile',
        logOutText: 'Log out',
        logOutHeadingText: 'Log out',
        logOutBodyText: 'Do you want to log out?',
        logOutCancelText: 'Cancel',
        logOutConfirmText: 'Yes, log out'
    },
    user: {
        id: '12345',
        fullNameText: 'Mock Name',
        initialsText: 'MN',
        image: {
            source: '/img.jpg',
            altText: 'Image of Mock Name'
        }
    },
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
