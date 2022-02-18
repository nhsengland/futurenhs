import { render, screen } from '@testing-library/react';

import { AriaLiveRegion } from './index';

import { Props } from './interfaces';

const testProps: Props = {
    children: <p>mock content</p>
};

describe('Aria live region', () => {

    it('renders children', () => {

        const props = Object.assign({}, testProps);

        render(<AriaLiveRegion {...props} />);

        expect(screen.getByText('mock content'));

    });
    
});
