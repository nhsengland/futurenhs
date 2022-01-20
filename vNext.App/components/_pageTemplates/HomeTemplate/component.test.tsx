import * as React from 'react';
import { render, screen } from '@testing-library/react';

import { HomeTemplate } from './index';
import { Props } from './interfaces';

const props: Props = {
    id: 'mockPageId',
    user: undefined,
    text: {
        title: 'Future NHS Home', 
        metaDescription: 'Your Future NHS home page',
        mainHeading: 'Latest discussions'
    }
}

describe('Home template', () => {

    it('renders correctly', () => {

        render(<HomeTemplate {...props} />);

        expect(screen.getByText('Latest discussions'));

    });
    
});
