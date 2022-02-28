import Head from 'next/head';

import { StandardLayout } from '@components/_pageLayouts/StandardLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { PageBody } from '@components/PageBody';

import { Props } from './interfaces';

/**
 * Home page template
 */
export const HomeTemplate: (props: Props) => JSX.Element = ({
    contentText,
    user,
    actions
}) => {

    const { title, 
            metaDescription, 
            mainHeading } = contentText ?? {};

    return (

        <StandardLayout 
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
                            <h1 className="nhsuk-heading-xl">{mainHeading}</h1>
                        </PageBody>
                    </LayoutColumn>
                </LayoutColumnContainer>
        </StandardLayout>

    )

}