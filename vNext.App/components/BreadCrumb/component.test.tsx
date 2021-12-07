import { render, screen } from '@testing-library/react';

import { BreadCrumb } from './index';

import { Props } from './interfaces';

const testProps: Props = {
    content: {
        descriptionHtml: 'mockContent'    
    },
    navMenuList: []
};

describe('BreadCrumb', () => {

    it('renders description Html', () => {

        const props = Object.assign({}, testProps);

        render(<BreadCrumb {...props} />);

        expect(screen.getByText('mockContent'));

    });
    
});
