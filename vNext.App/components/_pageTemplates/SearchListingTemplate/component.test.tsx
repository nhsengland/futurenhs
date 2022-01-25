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
        text: {
            title: 'Search',
            metaDescription: 'Search Future NHS',
            mainHeading: 'Searching'
        }
    };

    it('renders correctly', () => {

        render(<SearchListingTemplate {...props} />);

        expect(screen.getAllByText('Searching: mockTerm - 0 results found').length).toEqual(1);

    });
    
});
