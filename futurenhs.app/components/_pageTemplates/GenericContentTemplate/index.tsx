import Head from 'next/head';

import { StandardLayout } from '@components/_pageLayouts/StandardLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { PageBody } from '@components/PageBody';

import { Props } from './interfaces';

export const GenericContentTemplate: (props: Props) => JSX.Element = ({
    user,
    actions,
    contentText
}) => {

    const isAuthenticated: boolean = Boolean(user);

    const { metaDescription, 
            title, 
            mainHeading } = contentText ?? {};

    return (

        <StandardLayout
            shouldRenderSearch={isAuthenticated}
            shouldRenderUserNavigation={isAuthenticated}
            shouldRenderMainNav={isAuthenticated}
            user={user}
            actions={actions}
            className="u-bg-theme-3">
                <Head>
                    <title>{title}</title>
                    <meta name="description" content={metaDescription} />
                </Head>
                <LayoutColumnContainer>
                    <LayoutColumn>
                        <PageBody>
                            <h1>{mainHeading}</h1>
                        </PageBody>
                    </LayoutColumn>
                </LayoutColumnContainer>
        </StandardLayout>

    )

}
