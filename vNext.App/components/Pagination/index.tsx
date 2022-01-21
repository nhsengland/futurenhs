import { useState, useEffect } from 'react';
import Link from 'next/link';
import classNames from 'classnames';

import { SVGIcon } from '@components/SVGIcon';

import { Props } from './interfaces';

export const Pagination: (props: Props) => JSX.Element = ({
    id,
    text = {
        loadMore: 'Load more',
        previous: 'Previous',
        next: 'Next'
    },
    visiblePages = 3,
    pageNumber,
    pageSize,
    totalRecords,
    shouldEnableLoadMore,
    shouldDisable,
    getPageAction,
    className
}) => {

    const [isLoadMoreEnabled, setIsLoadMoreEnabled] = useState(false);

    const { loadMore, previous, next } = text;

    const queryString: string = `?pageNumber=`;
    const currentPaginationGroup: number = Math.ceil(pageNumber / visiblePages);
    const totalPages: number = Math.ceil(totalRecords / pageSize);
    const lowerRange: number = currentPaginationGroup * visiblePages - visiblePages + 1;
    const upperRange: number = (currentPaginationGroup * visiblePages > totalPages ? totalPages : currentPaginationGroup * visiblePages) + 1;
    const navItems: Array<any> = [];

    const generatedClasses = {
        wrapper: classNames('c-pagination', className),
        list: classNames('c-pagination_list', 'u-text-bold'),
        prevItem: classNames('c-pagination_item', 'c-pagination_item--prev'),
        nextItem: classNames('c-pagination_item', 'c-pagination_item--next'),
        link: classNames('c-pagination_link'),
        prevIcon: classNames('c-pagination_icon', 'u-mr-2'),
        nextIcon: classNames('c-pagination_icon', 'u-ml-2')
    }

    const handleLoadMore = (event) => {

        event.preventDefault();

        getPageAction?.({
            pageNumber: pageNumber + 1,
            pageSize: pageSize
        });

    };

    for(let i = lowerRange; i < upperRange; i++){

        const isActive: boolean = pageNumber === i;
        const generatedClasses = {
            item: classNames('c-pagination_item', {
                ['c-pagination_item--active']: isActive
            }),
            link: classNames('c-pagination_link'),
        }

        const navItem = (
            <li key={i} className={generatedClasses.item}>
                {isActive 
                
                    ?   <span aria-current="true" aria-label={`Current page, page ${i}`}>{i}</span> 
                    :   <Link href={`${queryString}${encodeURIComponent(i)}`} shallow={true}>
                            <a className={generatedClasses.link}>{i}</a>
                        </Link>
                    
                }
            </li>
        );

        navItems.push(navItem);

    }

    useEffect(() => {

        setIsLoadMoreEnabled(shouldEnableLoadMore);

    }, [shouldEnableLoadMore]);

    if(navItems.length < 2 || (isLoadMoreEnabled && pageNumber === totalPages)){

        return null;

    }

    if(isLoadMoreEnabled){

        return (

            <button disabled={shouldDisable} onClick={handleLoadMore} className="c-button c-button--secondary u-w-72">
                {loadMore}
            </button>

        )

    }

    return (

        <nav id={id} className={generatedClasses.wrapper} aria-label="Pagination"> 
            <p className="u-sr-only" aria-labelledby={id}>Pagination navigation</p>
            <ul className={generatedClasses.list}>
                {currentPaginationGroup > 1 &&
                    <li className={generatedClasses.prevItem}>
                        <a className={generatedClasses.link} href={`${queryString}${(currentPaginationGroup -1) * visiblePages}`}>
                            <SVGIcon name="icon-arrow-left" className={generatedClasses.prevIcon} />
                            {previous}<span className="u-sr-only"> set of pages</span>
                        </a>
                    </li>
                }
                {navItems}
                {((currentPaginationGroup * visiblePages) - visiblePages < totalPages - visiblePages) &&
                    <li className={generatedClasses.nextItem}>
                        <a className={generatedClasses.link} href={`${queryString}${(currentPaginationGroup * visiblePages) + 1}`}>
                            {next}<span className="u-sr-only"> set of pages</span>
                            <SVGIcon name="icon-arrow-right" className={generatedClasses.nextIcon} />
                        </a>
                    </li>
                }
            </ul>
        </nav>

    )

}
