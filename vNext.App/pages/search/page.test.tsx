import * as React from 'react';
import { render, screen } from '@testing-library/react';

import Page from './index.page';
import { Props } from '@components/_pageTemplates/SearchTemplate/interfaces';

describe('Search page', () => {

    const props: Props = {
        id: 'mockPageId',
        user: undefined,
        term: 'mockTerm',
        content: {
            titleText: 'Search',
            metaDescriptionText: 'Search Future NHS',
            mainHeadingHtml: 'Searching'
        }
    };

    it('renders correctly', () => {

        render(<Page {...props} />);

        expect(screen.getAllByText('Searching: mockTerm - 0 Results Found').length).toEqual(1);

    });
    
});
