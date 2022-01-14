import Head from 'next/head';

import { Link } from '@components/Link';
import { PaginationWithStatus } from '@components/PaginationWithStatus';
import { StandardLayout } from '@components/_pageLayouts/StandardLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';

import { Props } from './interfaces';

/**
 * Search listing template
 */
export const SearchListingTemplate: (props: Props) => JSX.Element = ({
    term,
    content,
    resultsList = [],
    pagination
}) => {

    const resultsCount: number = resultsList.length;
    const hasResults: boolean = resultsCount > 0;

    const { metaDescriptionText, 
            titleText, 
            mainHeadingHtml } = content ?? {};

    return (

        <StandardLayout className="u-bg-theme-3">
            <Head>
                <title>{titleText}</title>
                <meta name="description" content={metaDescriptionText} />
            </Head>
            <div className="u-px-4 u-py-10">
                <h1>{`${mainHeadingHtml}: ${term} - ${resultsCount} results found`}</h1>
                {!hasResults &&
                    <p>Sorry no results found</p>
                }
                {hasResults &&
                    <LayoutColumnContainer>
                        <LayoutColumn desktop={10}>
                            <ul>
                                {resultsList.map(({ example }, index) => {
    
                                    return (
                                    
                                        <li key={index}>{example}</li>
    
                                    )
    
                                })} 
                            </ul>
                            <PaginationWithStatus 
                                id="search-result-list-pagination"
                                shouldEnableLoadMore={false}
                                {...pagination} />
                        </LayoutColumn>
                    </LayoutColumnContainer>
                }
            </div>
        </StandardLayout>

    )

}
