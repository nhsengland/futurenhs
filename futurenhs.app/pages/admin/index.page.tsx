import { GetServerSideProps } from 'next'

import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { layoutIds } from '@constants/routes'
import { actions } from '@constants/actions'
import { withUser } from '@hofs/withUser'
import { withTextContent } from '@hofs/withTextContent'
import { withRoutes } from '@hofs/withRoutes'
import { selectPageProps } from '@selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'

import { AdminHomeTemplate } from '@components/_pageTemplates/AdminHomeTemplate'
import { Props } from '@components/_pageTemplates/AdminHomeTemplate/interfaces'

/**
 * Get props to inject into page on the initial server-side request
 */
 export const getServerSideProps: GetServerSideProps = async (context: GetServerSidePropsContext) => await pipeSSRProps(context, {
    routeId: '439794f2-9c58-4b6f-9fe8-d77a841e3055'
}, [
    withUser,
    withRoutes,
    withTextContent
], async (context: GetServerSidePropsContext) => {

    /**
     * Get data from request context
     */
    const props: Partial<Props> = selectPageProps(context);

    props.layoutId = layoutIds.ADMIN

    return {
        props,
        notFound: !props.actions.includes(actions.SITE_ADMIN_VIEW),
    }

});

/**
 * Export page template
 */
export default AdminHomeTemplate
