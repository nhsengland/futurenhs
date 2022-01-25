import Head from 'next/head';
import classNames from 'classnames';

import { Link } from '@components/Link';
import { PaginationWithStatus } from '@components/PaginationWithStatus';
import { StandardLayout } from '@components/_pageLayouts/StandardLayout';
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
    text,
    resultsList = [],
    pagination,
    // user
}) => {

    const resultsCount: number = resultsList.length;
    const hasResults: boolean = resultsCount > 0;

    const { metaDescription,
        title,
        mainHeading } = text ?? {};

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
            }
        });

        if(additionalSearchResults?.length){

            setSearchResultsList([...dynamicSearchResultsList, ...additionalSearchResults]);
            setPagination(pagination);

        }

    };

    const generatedClasses = {
        block: classNames('c-search-result', 'u-border-bottom-theme-8', 'u-mb-6', 'u-flex', 'u-flex-col'),
        header: classNames('c-search-result_header', 'u-text-theme-7', 'u-text-bold', 'u-order-1'),
        title: classNames('c-search-result_title', 'u-order-2','u-text-2xl'),
        body: classNames('c-search-result_body', 'u-p-0', 'o-truncated-text-lines-2', 'u-order-2')
    }

    const getGroupDetails = (item: SearchResult) => {
        let _item = item;
        while (_item.hasOwnProperty("meta") && _item.meta.type !== ContentType.GROUP) {
            _item = _item.meta
        }
        return _item.meta
    }
    const getMetaHeader = (item: SearchResult): JSX.Element => {

        const type: ContentType = item.type
        const parentType: ContentType = item.meta?.type;
        let _item: SearchResult = getGroupDetails(item) ?? item;
        //Discussion /groups/groupId/forum/discussionId
        // Files /groups/groupId/files/fileId
        // Folder /groups/groupId/folder/folderId
        //

        if (type === ContentType.DISCUSSION) {
            return <> Discussion on <Link href={`/groups/${_item.entityIds[_item.type+'Id']}`}>{_item.content.title}</Link> group forum </>;
        }
        else if (parentType === ContentType.DISCUSSION) {
            return <>Discussion on <Link href={`/groups/${_item.entityIds[_item.type+'Id']}`}>{_item.content.title}</Link> group forum</>;
        }
        else if (type === ContentType.GROUP) {
            return <>{capitalise()(item.type)}</>;
        }
        else {
            return <>{capitalise()(item.type)} on <Link href={`/groups/${_item.entityIds[_item.type+'Id']}`}>{_item.content.title}</Link> group</>;
        }

    }

    const getTitle = (item: SearchResult): JSX.Element => {
        const parentType: ContentType = item.meta?.type;

        if (parentType === ContentType.DISCUSSION) {
            return <Link href={`/groups/${item.meta.entityIds[item.meta.type+'Id']}/forum/${item.entityIds[item.type+'Id']}`}>
                <a>
                    <span>{capitalise()(item.type)} on discussion: </span>
                    <RichText wrapperElementType='span' bodyHtml={matchText()(item.meta.content.title, term)} />
                </a>
            </Link>
        }

        const url: string = `/groups/${item.meta.entityIds[item.meta.type+'Id']}` + (item.type === ContentType.GROUP? '':`/${item.type}s/${item.entityIds[item.type+'Id']}`)
        return <Link href={url}>
            <a>
                <RichText wrapperElementType='span' bodyHtml={matchText()(capitalise()(item.content.title), term)} />
            </a>
        </Link>

    }

    const formattedData = dynamicSearchResultsList.map((item: SearchResult): any => {

        /* Construct meta header */
        let metaHeader = getMetaHeader(item);
        /* Construct title */
        let title = getTitle(item);
        /* Construct body */
        let body = <RichText wrapperElementType='span' bodyHtml={matchText()(capitalise()(item.content.body), term)} />;

        return { metaHeader: metaHeader, title: title, body: body }
    })

    return (

        <StandardLayout className="u-bg-theme-3" searchTerm={term}>
            <Head>
                <title>{title}</title>
                <meta name="description" content={metaDescription} />
            </Head>
            <div className="u-px-4 u-py-10">
                <h1>{`${mainHeading}: ${term} - ${dynamicPagination.totalRecords} results found`}</h1>
                {!hasResults &&
                    <p>Sorry no results found</p>
                }
                {hasResults &&
                    <LayoutColumnContainer>
                        <LayoutColumn desktop={10}>
                            <ul className='u-p-0 u-list-none'>
                                {formattedData.map(({ metaHeader, title, body }, index) => {

                                    return (

                                        <li key={index} className={generatedClasses.block}>
                                            <h2 className={generatedClasses.title}>{title}</h2>
                                            <p className={generatedClasses.header}>{metaHeader}</p>
                                            <p className={generatedClasses.body}>{body}</p>
                                        </li>
                                    )

                                })}
                            </ul>
                            <PaginationWithStatus
                                id="search-result-list-pagination"
                                shouldEnableLoadMore={true}
                                getPageAction={handleGetPage}
                                totalRecords={dynamicPagination.totalRecords}
                                {...dynamicPagination} />
                        </LayoutColumn>
                    </LayoutColumnContainer>
                }
            </div>
        </StandardLayout>

    )

}
