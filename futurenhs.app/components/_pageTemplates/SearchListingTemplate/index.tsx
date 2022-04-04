import Head from 'next/head';
import classNames from 'classnames';

import { Link } from '@components/Link';
import { PaginationWithStatus } from '@components/PaginationWithStatus';
import { DynamicListContainer } from '@components/DynamicListContainer';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';

import { Props } from './interfaces';
import { useState } from 'react';
import { getSearchResults } from '@services/getSearchResults';
import { RichText } from '@components/RichText';
import { SearchResult } from '@appTypes/search';
import { ContentType } from '@appTypes/search-content';
import { matchText } from '@helpers/formatters/matchText';
import { capitalise } from '@helpers/formatters/capitalise';

/**
 * Search listing template
 */
export const SearchListingTemplate: (props: Props) => JSX.Element = ({
    term,
    minLength,
    contentText,
    resultsList = [],
    pagination
}) => {

    const resultsCount: number = resultsList.length;
    const hasResults: boolean = resultsCount > 0;
    const shouldEnableLoadMore: boolean = true;

    const { metaDescription,
        title,
        mainHeading, noResults, noResultsMinTermLength } = contentText ?? {};

    const noResultsMessage: string = (!term || term.length < minLength) ? noResultsMinTermLength : noResults;

    const [dynamicSearchResultsList, setSearchResultsList] = useState(resultsList);
    const [dynamicPagination, setPagination] = useState(pagination);

    const handleGetPage = async ({
        pageNumber: requestedPageNumber,
        pageSize: requestedPageSize
    }) => {

        const { data: additionalSearchResults, pagination } = await getSearchResults({
            term: term as string,
            pagination: {
                pageNumber: requestedPageNumber,
                pageSize: requestedPageSize
            },
            minLength: 3
        });

        if (additionalSearchResults?.length) {

            setSearchResultsList([...dynamicSearchResultsList, ...additionalSearchResults]);
            setPagination(pagination);
        }

    };

    const generatedClasses = {
        block: classNames('c-search-result', 'u-border-b-theme-8', 'u-mb-6', 'u-flex', 'u-flex-col', 'u-break-words'),
        header: classNames('c-search-result_header', 'u-text-theme-7', 'u-text-bold', 'u-order-1', 'o-truncated-text-lines-2'),
        title: classNames('c-search-result_title', 'u-order-2', 'nhsuk-heading-m', 'o-truncated-text-lines-2'),
        body: classNames('c-search-result_body', 'u-p-0', 'o-truncated-text-lines-2', 'u-order-2')
    }

    const getGroupDetails = (item: SearchResult) => {
        let _item = item;
        while (_item.hasOwnProperty("meta") && _item.meta.type !== ContentType.GROUP) {
            _item = _item.meta
        }
        return _item.meta
    }
    const getMetaHeader = ({ item, parentType, groupItem }): JSX.Element => {

        const resourceHref: string = `/groups/${groupItem.entityIds[groupItem.type + 'Id']}`;

        if (item.type === ContentType.DISCUSSION || parentType === ContentType.DISCUSSION) {
            return <> Discussion on <Link href={resourceHref}>{groupItem.content.title}</Link> group forum </>;
        }

        if (item.type === ContentType.GROUP) {
            return <>{capitalise({ value: item.type })}</>;
        }

        return <>{capitalise({ value: item.type })} on <Link href={resourceHref}>{groupItem.content.title}</Link> group</>;
    }

    const getTitle = ({ item, parentType, groupItem, stripHtmlPattern }): JSX.Element => {

        let resourceHref: string = `/groups/${groupItem.entityIds[groupItem.type + 'Id']}`

        if (parentType === ContentType.DISCUSSION || item.type === ContentType.DISCUSSION) {
            const isParentDiscussion: boolean = parentType === ContentType.DISCUSSION
            resourceHref += `/forum/${item.entityIds[item.type + 'Id']}`;
            return <Link href={resourceHref}>
                <a>
                    {isParentDiscussion && <span>{capitalise({ value: item.type })} on discussion: </span>}
                    <RichText wrapperElementType='span' stripHtmlPattern={stripHtmlPattern} bodyHtml={matchText({
                        value: item.content.title,
                        term: term
                    })} />
                </a>
            </Link>
        }

        resourceHref += (item.type === ContentType.GROUP ? '' : `/${item.type}s/${item.entityIds[item.type + 'Id']}`)

        return <Link href={resourceHref}>
            <a>
                <RichText wrapperElementType='span' stripHtmlPattern={stripHtmlPattern} bodyHtml={matchText({
                    value: capitalise({ value: item.content.title }),
                    term: term
                })} />
            </a>
        </Link>

    }

    const formattedData = dynamicSearchResultsList.map((item: SearchResult): any => {

        const parentType: ContentType = item.meta?.type;
        const groupItem: SearchResult = getGroupDetails(item) ?? item;
        const stripHtmlPattern: RegExp = new RegExp('(?!<\/?mark>)(<.*?>)|\&?nbsp;', 'gi');

        const options: any = {
            item: item,
            parentType: parentType,
            groupItem: groupItem,
            stripHtmlPattern: stripHtmlPattern
        }

        /* Construct meta header */
        const metaHeader: JSX.Element = getMetaHeader(options);
        /* Construct title */
        const title: JSX.Element = getTitle(options);

        /* Construct body */
        const body: JSX.Element = <RichText wrapperElementType='span' stripHtmlPattern={stripHtmlPattern} bodyHtml={matchText({
            value: capitalise({
                value: item.content.body
            }),
            term: term
        })} />

        return {
            metaHeader: metaHeader,
            title: title,
            body: body
        }

    })

    return (

        <>
            <Head>
                <meta name="description" content={metaDescription} />
            </Head>
            <div className="u-px-4 u-py-10">
                <h1 className="nhsuk-heading-xl">{`${mainHeading}: "${term ? term : ""}" - ${dynamicPagination?.totalRecords ?? 0} results found`}</h1>
                {!hasResults &&
                    <p>{noResultsMessage}</p>
                }
                {hasResults &&
                    <LayoutColumnContainer>
                        <LayoutColumn desktop={10}>
                            <DynamicListContainer
                                containerElementType="ul"
                                shouldEnableLoadMore={shouldEnableLoadMore}
                                className="u-p-0 u-list-none">
                                {formattedData.map(({ metaHeader, title, body }, index) => {

                                    return (

                                        <li key={index} className={generatedClasses.block}>
                                            <h2 className={generatedClasses.title}>{title}</h2>
                                            <p className={generatedClasses.header}>{metaHeader}</p>
                                            <p className={generatedClasses.body}>{body}</p>
                                        </li>
                                    )

                                })}
                            </DynamicListContainer>
                            <PaginationWithStatus
                                id="search-result-list-pagination"
                                shouldEnableLoadMore={shouldEnableLoadMore}
                                getPageAction={handleGetPage}
                                totalRecords={dynamicPagination.totalRecords}
                                {...dynamicPagination} />
                        </LayoutColumn>
                    </LayoutColumnContainer>
                }
            </div>
        </>

    )

}
