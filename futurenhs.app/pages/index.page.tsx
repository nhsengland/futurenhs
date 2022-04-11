import { GetServerSideProps } from 'next'

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withTextContent } from '@hofs/withTextContent'
import { GetServerSidePropsContext } from '@appTypes/next'

import { HomeTemplate } from '@components/_pageTemplates/HomeTemplate'
import { Props } from '@components/_pageTemplates/HomeTemplate/interfaces'

const routeId: string = '749bd865-27b8-4af6-960b-3f0458f8e92f'
const props: Partial<Props> = {}

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withUser({
    props,
    getServerSideProps: withRoutes({
        props,
        getServerSideProps: withTextContent({
            props,
            routeId: routeId,
            getServerSideProps: async (context: GetServerSidePropsContext) => {
                /**
                 * Return data to page template
                 */
                // return handleSSRSuccessProps({ props });

                /**
                 * Temporarily redirect to the groups index as the default homepage
                 * while purpose and content is established for this site index route
                 */
                return {
                    redirect: {
                        permanent: false,
                        destination: props.routes.groupsRoot,
                    },
                }
            },
        }),
    }),
})

/**
 * Export page template
 */
export default HomeTemplate
