import { GetServerSideProps } from 'next'
import { getSession } from 'next-auth/react'
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

export const getServerSideProps: GetServerSideProps = async (
    context: GetServerSidePropsContext
) =>
    await pipeSSRProps(
        context,
        {
            routeId: '043b4409-7aa4-4d9d-af9b-30cb0b469f02',
        },
        [withRoutes, withTextContent],
        async (context: GetServerSidePropsContext) => {
            const props: Partial<Props> = selectPageProps(context)
            const { query } = context
            const session = await getSession(context)

            /**
             * Set base template properties
             */
            ;(props as any).breadCrumbList = []
            ;(props as any).shouldRenderMainNav = false
            ;(props as any).className = 'u-bg-theme-3'

            /**
             * Redirect to site root if already signed out
             */
            if (session) {
                /**
                 * Get data from services
                 */
                try {
                    const callbackUrl: string = `${process.env.APP_URL}${context.resolvedUrl}`
                    const idTokenHint: string = session.id_token as string

                    /**
                     * Get next-auth specific csrf token and associated cookie header
                     */
                    const [csrfData] = await Promise.all([
                        getAuthCsrfData({ query }),
                    ])
                    const csrfToken: string = csrfData.data

                    /**
                     * Sign out locally
                     */
                    await fetchJSON(
                        `${process.env.APP_URL}${props.routes.authApiSignOut}`,
                        setFetchOpts({
                            method: requestMethods.POST,
                            body: { csrfToken, callbackUrl },
                        }),
                        defaultTimeOutMillis
                    )

                    /**
                     * Ensure the request to clear the token is passed through to the browser
                     */
                    context.res.setHeader(
                        'Set-Cookie',
                        'next-auth.session-token=; path=/; max-age=0'
                    )

                    /**
                     * Sign out on Azure
                     */
                    return {
                        redirect: {
                            permanent: false,
                            destination: `https://${process.env.AZURE_AD_B2C_TENANT_NAME}.b2clogin.com/${process.env.AZURE_AD_B2C_TENANT_NAME}.onmicrosoft.com/${process.env.AZURE_AD_B2C_PRIMARY_USER_FLOW}/oauth2/v2.0/logout?post_logout_redirect_uri=${callbackUrl}&id_token_hint=${idTokenHint}`,
                        },
                    }
                } catch (error) {
                    return handleSSRErrorProps({ props, error })
                }
            }

            /**
             * Return data to page template
             */
            return handleSSRSuccessProps({ props, context })
        }
    )

export default AuthSignOutPage
