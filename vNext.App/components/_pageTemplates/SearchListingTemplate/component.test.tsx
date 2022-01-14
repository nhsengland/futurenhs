import * as React from 'react';
import { render, screen } from '@testing-library/react';

import { SearchListingTemplate } from './index';
import { Props } from './interfaces';

describe('Search listing template', () => {

    const props: Props = {
        id: 'mockPageId',
        user: undefined,
        term: 'mockTerm',
        resultsList: [
            {
                example: '1'
            },
            {
                example: '2'
            },
            {
                example: '3'
            }
        ],
        content: {
            titleText: 'Search',
            metaDescriptionText: 'Search Future NHS',
            mainHeadingHtml: 'Searching'
        }
    };

    it('renders correctly', () => {

        render(<SearchListingTemplate {...props} />);

        expect(screen.getAllByText('Searching: mockTerm - 3 results found').length).toEqual(1);

    });
    
});
