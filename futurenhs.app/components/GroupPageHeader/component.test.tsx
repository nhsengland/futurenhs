import { render, screen } from '@testing-library/react';

import { GroupPageHeader } from './index';

import { Props } from './interfaces';

const testProps: Props = {
    id: 'mockId',
    text: {
        mainHeading: 'mockHeading',
        description: 'mockDescripton',
        navMenuTitle: 'mockNavMenuTitleText'   
    },
    navMenuList: []
};

describe('GroupPageHeader', () => {

    it('renders heading Html', () => {

        const props = Object.assign({}, testProps);

        render(<GroupPageHeader {...props} />);

        expect(screen.getByText('mockHeading'));

    });
    
});
