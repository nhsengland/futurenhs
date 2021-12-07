import * as React from 'react';
import { render, screen } from '@testing-library/react';

import Page from './index.page';

const props = {
    content: {
        titleText: 'Future NHS Home', 
        metaDescriptionText: 'Your Future NHS home page',
        mainHeadingHtml: 'Latest discussions'
    }
}

describe('Home page', () => {

    it('renders correctly', () => {

        render(<Page {...props} />);

        expect(screen.getByText('Latest discussions'));

    });
    
});
