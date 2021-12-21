import * as React from 'react';
import { render, screen } from '@testing-library/react';

import { SearchTemplate } from './index';
import { Props } from './interfaces';

describe('Search template', () => {

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

        render(<SearchTemplate {...props} />);

        expect(screen.getAllByText('Searching: mockTerm - 0 Results Found').length).toEqual(1);

    });
    
});
