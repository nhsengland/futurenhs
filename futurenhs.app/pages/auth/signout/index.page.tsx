import { GetServerSideProps } from 'next'

import { getSession } from 'next-auth/react';
import { getAuthCsrfData } from '@services/getAuthCsrfData';
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withTextContent } from '@hofs/withTextContent'
import { GetServerSidePropsContext } from '@appTypes/next'

import { AuthSignOutTemplate } from '@components/_pageTemplates/AuthSignOutTemplate';
import { Props } from '@components/_pageTemplates/AuthSignOutTemplate/interfaces';

const routeId: string = '043b4409-7aa4-4d9d-af9b-30cb0b469f02';
const props: Partial<Props> = {}

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withUser({
    props,
    isRequired: false,
    getServerSideProps: withRoutes({
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
                if (!session) {

                    return {
                        redirect: {
                            permanent: false,
                            destination: process.env.APP_URL,
                        },
                    }

                }

                /**
                 * Get data from services
                 */
                try {

                    /**
                     * Get next-auth specific csrf token and associated cookie header
                     */
                    const [csrfData] = await Promise.all([getAuthCsrfData({ query })]);

                    props.csrfToken = csrfData.data;

                    /**
                     * next-auth assumes a browser -> server request, so the returned set-cookie header needs
                     * passing through in order for the csrf token validation to succeed on form POST
                     */
                    context.res.setHeader('Set-Cookie', csrfData.headers.get('Set-Cookie'));

                } catch (error) {
                    return handleSSRErrorProps({ props, error })
                }

                /**
                 * Return data to page template
                 */
                return handleSSRSuccessProps({ props })
            },
        }),
    }),
})

/**
 * Export page template
 */
export default AuthSignOutTemplate
