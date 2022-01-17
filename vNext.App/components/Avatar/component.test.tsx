import { render, screen } from '@testing-library/react';

import { Avatar } from './index';

import { Props } from './interfaces';

const testProps: Props = {
    initials: 'mockInitials',
    image: null
};

describe('Avatar', () => {

    it('renders initials', () => {

        const props = Object.assign({}, testProps);

        render(<Avatar {...props} />);

        expect(screen.getByText('mockInitials'));

    });
    
});
