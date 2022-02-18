import { render, screen } from '@testing-library/react';

import { LayoutWidthContainer } from './index';

import { Props } from './interfaces';

const testProps: Props = {
};

describe('Layout Width Container', () => {

    it('renders child content', () => {

        const props = Object.assign({}, testProps);

        render(<LayoutWidthContainer {...props} ><p>Mock content</p></LayoutWidthContainer>);

        expect(screen.getByText('Mock content'));

    });
    
});
