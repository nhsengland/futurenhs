import { render, screen } from '@testing-library/react';

import { PageHeader } from './index';

import { Props } from './interfaces';

const testProps: Props = {
    id: 'mockId',
    content: {
        mainHeadingHtml: 'mockHeading',
        descriptionHtml: 'mockDescripton',
        navMenuTitleText: 'mockNavMenuTitleText'   
    },
    navMenuList: []
};

describe('PageHeader', () => {

    it('renders heading Html', () => {

        const props = Object.assign({}, testProps);

        render(<PageHeader {...props} />);

        expect(screen.getByText('mockHeading'));

    });
    
});
