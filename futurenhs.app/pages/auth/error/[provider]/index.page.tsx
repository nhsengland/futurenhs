import { GetServerSideProps } from 'next'
import { getProviders } from 'next-auth/react'
//import getAuthorizationUrl from 'next-auth/core/lib/oauth/authorization-url'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import authConfig from '@pages/api/auth/[...nextauth].page'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { GetServerSidePropsContext } from '@appTypes/next'
import { Props } from '@components/_pageTemplates/AuthSignInTemplate/interfaces'
import { selectPageProps } from '@selectors/context'

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (
    context: GetServerSidePropsContext
) =>
    await pipeSSRProps(
        context,
        {},
        [[withUser, { isRequired: false }], withRoutes],
        async (context: GetServerSidePropsContext) => {
            const props: Partial<Props> = selectPageProps(context)
            const providers: any = await getProviders()

            console.log(providers)
            //const authorizationUrl: any = await getAuthorizationUrl();
            return {
                redirect: {
                    permanent: false,
                    destination:
                        'https://mtb2cld.b2clogin.com/MTB2CLD.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1_FNHSLocalDev&client_id=1b8ed2d8-0324-4be9-ac94-38d08043601a&nonce=defaultNonce&redirect_uri=http%3A%2F%2Flocalhost%3A5000&scope=openid&response_type=code&prompt=login',
                },
            }

            /**
             * Return data to page template
             */
            return handleSSRSuccessProps({ props, context })
        }
    )

/**
 * Export page template
 */
export default () => {}
