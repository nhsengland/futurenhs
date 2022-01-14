import * as React from 'react';
import { render, screen } from '@testing-library/react';

import { SearchListingTemplate } from './index';
import { Props } from './interfaces';

describe('Search listing template', () => {

    const props: Props = {
        id: 'mockPageId',
        user: undefined,
        term: 'mockTerm',
        resultsList: [],
        content: {
            titleText: 'Search',
            metaDescriptionText: 'Search Future NHS',
            mainHeadingHtml: 'Searching'
        }
    };

    it('renders correctly', () => {

        render(<SearchListingTemplate {...props} />);

        expect(screen.getAllByText('Searching: mockTerm - 0 Results Found').length).toEqual(1);

    });
    
});
