import Head from 'next/head';

import { StandardLayout } from '@components/_pageLayouts/StandardLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';

import { Props } from './interfaces';

export const GenericContentTemplate: (props: Props) => JSX.Element = ({
    user,
    content
}) => {

    const isAuthenticated: boolean = Boolean(user);

    const { metaDescriptionText, 
            titleText, 
            mainHeadingHtml } = content ?? {};

    return (

        <StandardLayout
            shouldRenderSearch={isAuthenticated}
            shouldRenderUserNavigation={isAuthenticated}
            shouldRenderMainNav={isAuthenticated}
            className="u-bg-theme-3">
                <Head>
                    <title>{titleText}</title>
                    <meta name="description" content={metaDescriptionText} />
                </Head>
                <LayoutColumnContainer className="u-py-10">
                    <h1>{mainHeadingHtml}</h1>
                </LayoutColumnContainer>
        </StandardLayout>

    )

}
