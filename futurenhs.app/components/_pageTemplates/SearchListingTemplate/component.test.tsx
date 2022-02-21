import * as React from 'react';
import { render, screen } from '@testing-library/react';

import { SearchListingTemplate } from './index';
import { Props } from './interfaces';
// TODO
describe('Search listing template', () => {

    const props: Props = {
        id: 'mockPageId',
        user: undefined,
        term: 'mockTermTestingItRandomlyToGet0Results',
        minLength: 3,
        resultsList: [],
        contentText: {
            title: 'Search',
            metaDescription: 'Search Future NHS',
            mainHeading: 'Searching'
        },
    };

    it('renders correctly', () => {

        render(<SearchListingTemplate {...props} />);
        const { metaDescription,
            title,
            mainHeading } = props.contentText ?? {};
        expect(screen.getAllByText(`${mainHeading}: ${props.term} - ${props.resultsList.length} results found`).length).toEqual(1);

    });

});
