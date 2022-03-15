import * as React from 'react';
import { cleanup, render, screen } from '@testing-library/react';
import * as nextRouter from 'next/router';

import { routes } from '@jestMocks/generic-props';
import { SearchListingTemplate } from './index';
import { Props } from './interfaces';

describe('Search listing template', () => {

    (nextRouter as any).useRouter = jest.fn();
    (nextRouter as any).useRouter.mockImplementation(() => ({
        asPath: '/search',
    }));

    const props: Props = {
        id: 'mockPageId',
        routes: routes,
        user: undefined,
        term: 'mockTermTestingItRandomlyToGet0Results',
        minLength: 3,
        resultsList: [],
        contentText: {
            title: 'Search',
            metaDescription: 'Search Future NHS',
            mainHeading: 'Searching',
            noResults: 'Mock no results found',
            noResultsMinTermLength: 'Mock no results minimum term length'
        },
    };

    it('renders correctly', () => {

        render(<SearchListingTemplate {...props} />);
        const { metaDescription,
            title,
            mainHeading } = props.contentText ?? {};
        expect(screen.getAllByText(`${mainHeading}: "${props.term}" - ${props.resultsList.length} results found`).length).toEqual(1);

    });

    it('conditionally renders search results if results are found/resultsList is not empty', () => {

        render(<SearchListingTemplate {...props} />);

        expect(screen.getAllByText('Mock no results found').length).toBe(1);

        cleanup();

        const propsCopy: Props = Object.assign({}, props, {
            resultsList: [
                {
                    type: "group",
                    entityIds: { groupId: "Mock entity" },
                    content: {
                        title: "Mock search result title",
                        body: "Mock search result body"
                    },
                    meta: {
                        type: "group",
                        entityIds: { groupId: "Mock entity" },
                        content: {
                            title: "Mock search result title",
                            body: "Mock search result body"
                        }
                    }
                }
            ],
            pagination: {
                totalRecords: 1
            }
        })

        render(<SearchListingTemplate {...propsCopy} />);

        expect(screen.getAllByText('Mock search result body').length).toBe(1);

    });

    it('renders minimum required term length message if search term is less than minimum length', () => {

        const propsCopy: Props = Object.assign({}, props, { term: 'no' });

        render(<SearchListingTemplate {...propsCopy} />);

        expect(screen.getAllByText('Mock no results minimum term length').length).toBe(1);

    });

});
