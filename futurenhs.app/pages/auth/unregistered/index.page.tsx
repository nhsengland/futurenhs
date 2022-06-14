import { GetServerSideProps } from 'next'

import { getSession } from 'next-auth/react';
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { withRoutes } from '@hofs/withRoutes'
import { withTextContent } from '@hofs/withTextContent'
import { GetServerSidePropsContext } from '@appTypes/next'

import { AuthUnregisteredTemplate } from '@components/_pageTemplates/AuthUnregisteredTemplate';
import { Props } from '@components/_pageTemplates/AuthUnregisteredTemplate/interfaces';

const routeId: string = '426b71e4-aa63-4386-a3ab-0769173f6456';
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
                        destination: `${process.env.APP_URL}/auth/signin`,
                    },
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
export default AuthUnregisteredTemplate
