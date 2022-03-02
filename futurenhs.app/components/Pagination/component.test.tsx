import React from 'react';
import { render, screen, cleanup } from '@testing-library/react';
import * as nextRouter from 'next/router';

import { Pagination } from './index';
import { Props } from './interfaces';

describe('Pagination', () => {

    (nextRouter as any).useRouter = jest.fn();
    (nextRouter as any).useRouter.mockImplementation(() => ({ 
        query: {
            pageQuery: ""
        } 
    }));
    
    const props: Props = {
        id: 'Mock id',
        pageNumber: 2,
        pageSize: 10,
        totalRecords: 50,
        shouldEnableLoadMore: false,
    }

    it('renders correctly', () => {

        render(<Pagination {...props}/>);

        expect(screen.getAllByText('Next').length).toBe(1);
        
    });

    it('does not render either pagination method if there is only one page', () => {

        const propsCopy: Props = Object.assign({}, props, {
            pageNumber: 1,
            totalRecords: 10
        });

        render(<Pagination {...propsCopy}/>);

        expect(screen.queryByText('Load more')).toBeNull();
        expect(screen.queryByText('Next')).toBeNull();

    });

    it('renders load more button if shouldEnableLoadMore is true. Does not render if there are no more pages.', () => {
        
        const propsCopy: Props = Object.assign({}, props, {
            shouldEnableLoadMore: true
        });

        render(<Pagination {...propsCopy}/>);
        
        expect(screen.getAllByText('Load more').length).toBe(1);
        expect(screen.queryByText('Next')).toBeNull();

        cleanup();

        propsCopy.pageNumber = 5;

        render(<Pagination {...propsCopy}/>);

        expect(screen.queryByText('Load more')).toBeNull();
    });

    it('renders passed button text values or defaults if not provided', () => {


        render(<Pagination {...props}/>);

        expect(screen.getAllByText('Previous').length).toBe(1);
        expect(screen.getAllByText('Next').length).toBe(1);

        cleanup();

        const propsCopy: Props = Object.assign({}, props, {
            text: {
                previous: 'Mock previous',
                next: 'Mock next',
            }
        });

        render(<Pagination {...propsCopy}/>);

        expect(screen.getAllByText('Mock previous').length).toBe(1);
        expect(screen.getAllByText('Mock next').length).toBe(1);

    });

});