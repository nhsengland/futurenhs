import { GetServerSideProps } from 'next'

import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withTextContent } from '@hofs/withTextContent'
import { selectPageProps } from '@selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'

import { HomeTemplate } from '@components/_pageTemplates/HomeTemplate'
import { Props } from '@components/_pageTemplates/HomeTemplate/interfaces'

/**
 * Get props to inject into page on the initial server-side request
 */
 export const getServerSideProps: GetServerSideProps = async (context: GetServerSidePropsContext) => await pipeSSRProps(context, {
    routeId: '749bd865-27b8-4af6-960b-3f0458f8e92f'
}, [
    withUser,
    withRoutes,
    withTextContent
], async (context: GetServerSidePropsContext) => {

    /**
     * Get data from request context
     */
    const props: Partial<Props> = selectPageProps(context);

    return {
        redirect: {
            permanent: false,
            destination: props.routes.groupsRoot,
        },
    }

});

/**
 * Export page template
 */
export default HomeTemplate
