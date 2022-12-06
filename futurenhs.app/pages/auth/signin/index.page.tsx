import { GetServerSideProps } from 'next'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { getAuthCsrfData } from '@services/getAuthCsrfData'
import { selectQuery } from '@helpers/selectors/context'
import { handleSSRAuthSuccessProps } from '@helpers/util/ssr/handleSSRAuthSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { withRoutes } from '@helpers/hofs/withRoutes'
import { withTextContent } from '@helpers/hofs/withTextContent'
import { selectPageProps } from '@helpers/selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { PageBody } from '@components/layouts/PageBody'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { RichText } from '@components/generic/RichText'
import { Page } from '@appTypes/page'
import { GenericPageTextContent } from '@appTypes/content'
import { authOptions } from '@pages/api/auth/[...nextauth].page'
import { unstable_getServerSession } from 'next-auth'
import SignInSubmitButton from '@components/forms/SignInSubmitButton'
import { getFeatureEnabled } from '@services/getFeatureEnabled'
import { features } from '@constants/routes'

interface ContentText extends GenericPageTextContent {
    signUpHtml: string
    signUpHeading: string
}

export interface Props extends Page {
    contentText: ContentText
    selfRegisterEnabled: boolean
    b2cSignUpUrl: string
}

/**
 * Auth signin template
 */
const AuthSignInPage: (props: Props) => JSX.Element = ({
    csrfToken,
    routes,
    contentText,
    b2cSignUpUrl,
    selfRegisterEnabled,
}) => {
    const { authApiSignInAzureB2C } = routes ?? {}
    const {
        mainHeading,
        secondaryHeading,
        intro,
        bodyHtml,
        signUpHtml,
        signUpHeading,
    } = contentText ?? {}

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
                    <SignInSubmitButton
                        submitText="Sign In"
                        csrfToken={csrfToken}
                        action={authApiSignInAzureB2C}
                    />
                    {!selfRegisterEnabled
                        ? secondaryHeading && (
                              <h2 className="nhsuk-heading-l">
                                  {secondaryHeading}
                              </h2>
                          )
                        : signUpHeading && (
                              <h2 className="nhsuk-heading-l">
                                  {signUpHeading}
                              </h2>
                          )}
                    {!selfRegisterEnabled
                        ? bodyHtml && (
                              <RichText
                                  wrapperElementType="div"
                                  className="u-mb-10"
                                  bodyHtml={bodyHtml}
                              />
                          )
                        : signUpHtml && (
                              <RichText
                                  wrapperElementType="div"
                                  className="u-mb-10"
                                  bodyHtml={signUpHtml.replace(
                                      '%SIGNUP_URL%',
                                      b2cSignUpUrl
                                  )}
                              />
                          )}
                </LayoutColumn>
            </LayoutColumnContainer>
        </PageBody>
    )
}

export const getServerSideProps: GetServerSideProps = async (
    context: GetServerSidePropsContext
) => {
    return await pipeSSRProps(
        context,
        {
            routeId: '46524db4-44eb-4296-964d-69dfc2279f01',
        },
        [withRoutes, withTextContent],
        async (context: GetServerSidePropsContext) => {
            const props: Partial<Props> = selectPageProps(context)
            const { query } = context
            const error: string = selectQuery(context, 'error')
            const session = await unstable_getServerSession(
                context.req,
                context.res,
                authOptions
            )

            /**
             * Set base template properties
             */
            ;(props as any).breadCrumbList = []
            ;(props as any).shouldRenderMainNav = false
            ;(props as any).className = 'u-bg-theme-3'

            props.selfRegisterEnabled = false

            try {
                const response = await getFeatureEnabled({
                    slug: features.SELF_REGISTER,
                })
                props.selfRegisterEnabled = response.data
            } catch (e) {}

            /**
            }

            props.selfRegisterEnabled = 
             * Redirect to site root if already signed in
             */
            if (session) {
                return {
                    redirect: {
                        permanent: false,
                        destination: `${process.env.APP_URL}/groups`,
                    },
                }
            }

            /**
             * Handle any returned OAuth errors
             */
            if (error) {
                props.errors = [
                    {
                        [500]: error,
                    },
                ]

                return handleSSRAuthSuccessProps({ props, context })
            }

            /**
             * Get data from services
             */
            try {
                /**
                 * Get next-auth specific csrf token and associated cookie header
                 */
                const [csrfData] = await Promise.all([
                    getAuthCsrfData({ query }),
                ])
                props.csrfToken = csrfData.data
                const callbackUrl: string = `${process.env.APP_URL}/api/auth/signin`
                props.b2cSignUpUrl = `https://${process.env.AZURE_AD_B2C_TENANT_NAME}.b2clogin.com/${process.env.AZURE_AD_B2C_TENANT_NAME}.onmicrosoft.com/${process.env.AZURE_AD_B2C_SIGNUP_USER_FLOW}/oauth2/v2.0/authorize?client_id=${process.env.AZURE_AD_B2C_CLIENT_ID}&scope=offline_access%20openid&response_type=code&redirect_uri=${callbackUrl}&prompt=login`

                /**
                 * next-auth assumes a browser -> server request, so the returned set-cookie header needs
                 * passing through in order for the csrf token validation to succeed on form POST
                 */
                context.res.setHeader(
                    'Set-Cookie',
                    csrfData.headers.get('Set-Cookie')
                )
            } catch (error) {
                return handleSSRErrorProps({ props, error })
            }

            /**
             * Return data to page template
             */
            return handleSSRAuthSuccessProps({ props, context })
        }
    )
}

export default AuthSignInPage
function getFeatureFlag(arg0: {
    slug: features
}): { data: { enabled: any } } | PromiseLike<{ data: { enabled: any } }> {
    throw new Error('Function not implemented.')
}
