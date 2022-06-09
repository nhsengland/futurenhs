import { GetServerSideProps } from 'next'

import { getSession } from 'next-auth/react';
import { getAuthCsrfData } from '@services/getAuthCsrfData';
import { selectQuery } from '@selectors/context'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { withRoutes } from '@hofs/withRoutes'
import { withTextContent } from '@hofs/withTextContent'
import { GetServerSidePropsContext } from '@appTypes/next'

import { AuthSignInTemplate } from '@components/_pageTemplates/AuthSignInTemplate';
import { Props } from '@components/_pageTemplates/AuthSignInTemplate/interfaces';

const routeId: string = '46524db4-44eb-4296-964d-69dfc2279f01';
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
            const error: string = selectQuery(context, 'error');
            const session = await getSession(context);

            /**
             * Hide breadcrumbs
             */
            (props as any).breadCrumbList = [];

            /**
             * Redirect to site root if already signed in
             */
            if(session){

                return {
                    redirect: {
                        permanent: false,
                        destination: process.env.APP_URL,
                    }, 
                }

            }

            /**
             * Handle any returned OAuth errors
             */
            if (error) {

                props.errors = [{
                    [500]: error
                }];

                return handleSSRSuccessProps({ props })

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
    })
})

/**
 * Export page template
 */
export default AuthSignInTemplate
