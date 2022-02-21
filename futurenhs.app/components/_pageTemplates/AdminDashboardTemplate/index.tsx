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

/**
 * Admin dashboard template
 */
export const AdminDashboardTemplate: (props: Props) => JSX.Element = ({
    contentText,
    user,
    actions
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
            actions={actions}
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
