import { GetServerSideProps } from 'next';
import { getSession } from 'next-auth/react';

import { setFetchOpts, fetchJSON } from '@helpers/fetch';
import { getAuthCsrfData } from '@services/getAuthCsrfData';
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { withRoutes } from '@hofs/withRoutes'
import { withTextContent } from '@hofs/withTextContent'
import { GetServerSidePropsContext } from '@appTypes/next'

import { AuthSignOutTemplate } from '@components/_pageTemplates/AuthSignOutTemplate';
import { Props } from '@components/_pageTemplates/AuthSignOutTemplate/interfaces';
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch';

const routeId: string = '043b4409-7aa4-4d9d-af9b-30cb0b469f02';
const props: Partial<Props> = {}

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withRoutes({
    props,
    getServerSideProps: withTextContent({
        props,
        routeId,
        getServerSideProps: async (context: GetServerSidePropsContext) => {

            const { query } = context;
            const session = await getSession(context);

            /**
             * Hide breadcrumbs
             */
            (props as any).breadCrumbList = [];

            /**
             * Redirect to site root if already signed out
             */
            if (session) {

                /**
                 * Get data from services
                 */
                try {

                    const callbackUrl: string = `${process.env.APP_URL}${context.resolvedUrl}`;
                    const idTokenHint: string = (session.id_token as string);

                    /**
                     * Get next-auth specific csrf token and associated cookie header
                     */
                    const [csrfData] = await Promise.all([getAuthCsrfData({ query })]);
                    const csrfToken: string = csrfData.data;

                    /**
                     * Sign out locally
                     */
                    await fetchJSON(
                        `${process.env.APP_URL}${props.routes.authApiSignOut}`,
                        setFetchOpts({
                            method: requestMethods.POST,
                            body: { csrfToken, callbackUrl }
                        }),
                        defaultTimeOutMillis
                    )

                    /**
                     * Ensure the request to clear the token is passed through to the browser
                     */
                    context.res.setHeader('Set-Cookie', 'next-auth.session-token=; path=/; max-age=0');

                    /**
                     * Sign out on Azure
                     */
                    return {
                        redirect: {
                            permanent: false,
                            destination: `https://${process.env.AZURE_AD_B2C_TENANT_NAME}.b2clogin.com/${process.env.AZURE_AD_B2C_TENANT_NAME}.onmicrosoft.com/${process.env.AZURE_AD_B2C_PRIMARY_USER_FLOW}/oauth2/v2.0/logout?post_logout_redirect_uri=${callbackUrl}&id_token_hint=${idTokenHint}`,
                        }
                    }

                } catch (error) {
                    return handleSSRErrorProps({ props, error })
                }

            }

            /**
             * Return data to page template
             */
            return handleSSRSuccessProps({ props })
        },
    }),
})

/**
 * Export page template
 */
export default AuthSignOutTemplate
