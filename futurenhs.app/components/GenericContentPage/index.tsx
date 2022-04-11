import Head from 'next/head'

import { StandardLayout } from '@components/_pageLayouts/StandardLayout'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'

import { Props } from './interfaces'

export const GenericContentPage: (props: Props) => JSX.Element = ({
    isAuthenticated,
    text,
}) => {
    const { metaDescription, title, mainHeading } = text ?? {}

    return (
        <StandardLayout
            shouldRenderSearch={isAuthenticated}
            shouldRenderUserNavigation={isAuthenticated}
            shouldRenderMainNav={isAuthenticated}
        >
            <Head>
                <title>{title}</title>
                <meta name="description" content={metaDescription} />
            </Head>
            <LayoutColumnContainer className="u-py-10">
                <h1>{mainHeading}</h1>
            </LayoutColumnContainer>
        </StandardLayout>
    )
}
