import Head from 'next/head';

import { StandardLayout } from '@components/_pageLayouts/StandardLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';

import { Props } from './interfaces';

/**
 * Home page template
 */
export const HomeTemplate: (props: Props) => JSX.Element = ({
    content,
    user
}) => {

    const { titleText, 
            metaDescriptionText, 
            mainHeadingHtml } = content ?? {};

    return (

        <StandardLayout user={user} className="u-bg-theme-3">
            <Head>
                <title>{titleText}</title>
                <meta name="description" content={metaDescriptionText} />
            </Head>
            <LayoutColumnContainer>
                <LayoutColumn className="u-px-4 u-py-10">
                    <h1>{mainHeadingHtml}</h1>
                </LayoutColumn>
            </LayoutColumnContainer>
        </StandardLayout>

    )

}