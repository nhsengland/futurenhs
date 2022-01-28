import Head from 'next/head';

import { StandardLayout } from '@components/_pageLayouts/StandardLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';

import { Props } from './interfaces';

/**
 * Home page template
 */
export const HomeTemplate: (props: Props) => JSX.Element = ({
    contentText,
    user
}) => {

    const { title, 
            metaDescription, 
            mainHeading } = contentText ?? {};

    return (

        <StandardLayout user={user} className="u-bg-theme-3">
            <Head>
                <title>{title}</title>
                <meta name="description" content={metaDescription} />
            </Head>
            <LayoutColumnContainer>
                <LayoutColumn className="u-px-4 u-py-10">
                    <h1>{mainHeading}</h1>
                </LayoutColumn>
            </LayoutColumnContainer>
        </StandardLayout>

    )

}