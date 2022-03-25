import { render, screen } from '@testing-library/react';

import { Avatar } from './index';

import { Props } from './interfaces';

const testProps: Props = {
    initials: 'mockInitials',
    image: null
};

describe('Avatar', () => {

    it('renders initials', () => {

        const props: Props = Object.assign({}, testProps);

        render(<Avatar {...props} />);

        expect(screen.getByText('mockInitials'));

    });

    it('renders image', () => {

        const props: Props = Object.assign({}, testProps, {
            image: {
                src: "https://www.google.com",
                height: 250,
                width: 250,
                altText: "Mock alt text"
            }
        });
        
        render(<Avatar {...props} />);

        expect(screen.getAllByAltText('Mock alt text').length).toBe(1);




    });
    
});
