import Head from 'next/head'

import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { PageBody } from '@components/PageBody'

import { Props } from './interfaces'

/**
 * Home page template
 */
export const HomeTemplate: (props: Props) => JSX.Element = ({
    contentText,
}) => {
    const { title, metaDescription, mainHeading } = contentText ?? {}

    return (
        <>
            <Head>
                <meta name="description" content={metaDescription} />
            </Head>
            <LayoutColumnContainer>
                <LayoutColumn>
                    <PageBody>
                        <h1 className="nhsuk-heading-xl">{mainHeading}</h1>
                    </PageBody>
                </LayoutColumn>
            </LayoutColumnContainer>
        </>
    )
}
