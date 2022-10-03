import { GetServerSideProps } from 'next'
import { getSession, signOut } from 'next-auth/react'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { setFetchOpts, fetchJSON } from '@helpers/fetch'
import { getAuthCsrfData } from '@services/getAuthCsrfData'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { withRoutes } from '@hofs/withRoutes'
import { withTextContent } from '@hofs/withTextContent'
import { GetServerSidePropsContext } from '@appTypes/next'
import { selectPageProps } from '@selectors/context'
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch'
import { PageBody } from '@components/PageBody'
import { RichText } from '@components/RichText'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import Link from 'next/link'

import { Page } from '@appTypes/page'
import { GenericPageTextContent } from '@appTypes/content'
import { routes } from '@constants/routes'

interface ContentText extends GenericPageTextContent {
    signIn: string
}

interface Props extends Page {
    contentText: ContentText
}

/**
 * Auth signout template
 */
const AuthSignOutPage: (props: Props) => JSX.Element = ({
    routes,
    contentText,
}) => {
    const { authSignIn } = routes ?? {}
    const { mainHeading, intro, signIn } = contentText ?? {}

    return (
        <PageBody className="tablet:u-px-0">
            <LayoutColumnContainer justify="centre">
                <LayoutColumn tablet={8} desktop={6}>
                    {mainHeading && (
                        <h1 className="nhsuk-heading-xl">{mainHeading}</h1>
                    )}
                    {intro && (
                        <RichText
                            wrapperElementType="div"
                            className="u-mb-8"
                            bodyHtml={intro}
                        />
                    )}
                    {authSignIn && signIn && (
                        <Link href={authSignIn}>
                            <a className="c-button">{signIn}</a>
                        </Link>
                    )}
                </LayoutColumn>
            </LayoutColumnContainer>
        </PageBody>
    )
}

export const getServerSideProps = async (
    context: GetServerSidePropsContext
) => {
    const signInPage = `${process.env.APP_URL}${routes.SIGN_IN}`

    await signOut({ callbackUrl: signInPage })
}

export default AuthSignOutPage
