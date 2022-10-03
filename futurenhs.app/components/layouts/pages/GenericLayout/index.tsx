import classNames from 'classnames'
import Head from 'next/head'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { PageBody } from '@components/layouts/PageBody'
import { RichText } from '@components/generic/RichText'
import { Page } from '@appTypes/page'
import { User } from '@appTypes/user'

export interface Props extends Page {
    user: User
    pageTitle?: string
}

const GenericLayout: (props: Props) => JSX.Element = ({
    user,
    contentText,
    pageTitle,
}) => {
    const isAuthenticated: boolean = Boolean(user)

    const { metaDescription, title, mainHeading, bodyHtml } = contentText ?? {}

    return (
        <>
            <Head>
                <meta name="description" content={metaDescription} />
            </Head>
            <LayoutColumnContainer>
                <LayoutColumn>
                    <PageBody className="tablet:u-px-0">
                        <h1 className="nhsuk-heading-xl">{mainHeading}</h1>
                        <RichText bodyHtml={bodyHtml} />
                    </PageBody>
                </LayoutColumn>
            </LayoutColumnContainer>
        </>
    )
}

export default GenericLayout
