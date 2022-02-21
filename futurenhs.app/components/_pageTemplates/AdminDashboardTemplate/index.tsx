import Head from 'next/head';
import classNames from 'classnames';

import { Link } from '@components/Link';
import { AriaLiveRegion } from '@components/AriaLiveRegion';
import { PaginationWithStatus } from '@components/PaginationWithStatus';
import { StandardLayout } from '@components/_pageLayouts/StandardLayout';
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
 * Admin dashboard template
 */
export const AdminDashboardTemplate: (props: Props) => JSX.Element = ({
    contentText,
    user
}) => {

    const shouldEnableLoadMore: boolean = true;

    const { metaDescription,
            title,
            mainHeading } = contentText ?? {};


    const generatedClasses = {

    }

    return (

        <StandardLayout
            user={user}
            className="u-bg-theme-3">
                <Head>
                    <title>{title}</title>
                    <meta name="description" content={metaDescription} />
                </Head>
            <div className="u-px-4 u-py-10">
                <h1>{mainHeading}</h1>
            </div>
        </StandardLayout>

    )

}
