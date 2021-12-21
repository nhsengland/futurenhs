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
    content,
    logOutUrl
}) => {

    const { titleText, 
            metaDescriptionText, 
            mainHeadingHtml } = content ?? {};

    return (

        <StandardLayout 
            shouldRenderSearch={false}
            shouldRenderUserNavigation={false}
            shouldRenderMainNav={false}
            className="u-bg-theme-3">
                <Head>
                    <title>{titleText}</title>
                    <meta name="description" content={metaDescriptionText} />
                </Head>
                <LayoutColumnContainer justify="centre">
                    <LayoutColumn tablet={6} className="u-py-10">
                        <h1>{mainHeadingHtml}</h1>
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
