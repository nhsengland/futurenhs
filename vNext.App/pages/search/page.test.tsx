import * as React from 'react';
import { render, screen } from '@testing-library/react';

import Page from './index.page';
import { Props } from '@components/_pageTemplates/SearchListingTemplate/interfaces';

describe('Search page', () => {

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

        render(<Page {...props} />);

        expect(screen.getAllByText('Searching: mockTerm - 0 results found').length).toEqual(1);

    });
    
});
