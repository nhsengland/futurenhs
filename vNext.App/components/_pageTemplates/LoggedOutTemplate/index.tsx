import Head from 'next/head';

import { Link } from '@components/Link';
import { StandardLayout } from '@components/_pageLayouts/StandardLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';

import { Props } from './interfaces';

/**
 * Logged out template
 */
export const LoggedOutTemplate: (props: Props) => JSX.Element = ({
    contentText,
    logOutUrl
}) => {

    const { title, 
            metaDescription, 
            mainHeading } = contentText ?? {};

    return (

        <StandardLayout 
            shouldRenderSearch={false}
            shouldRenderUserNavigation={false}
            shouldRenderMainNav={false}
            className="u-bg-theme-3">
                <Head>
                    <title>{title}</title>
                    <meta name="description" content={metaDescription} />
                </Head>
                <LayoutColumnContainer justify="centre">
                    <LayoutColumn tablet={6} className="u-py-10">
                        <h1>{mainHeading}</h1>
                        <p>Your are now logged out</p>
                        <p className="desktop:u-pb-4">
                            <Link href={logOutUrl}>
                                <a>Log in again</a>
                            </Link>
                        </p>
                    </LayoutColumn>
                </LayoutColumnContainer>
        </StandardLayout>

    )

}
