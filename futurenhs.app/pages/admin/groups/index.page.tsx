import { GetServerSideProps } from 'next'

import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { layoutIds } from '@constants/routes'
import { actions } from '@constants/actions'
import { withUser } from '@hofs/withUser'
import { withTextContent } from '@hofs/withTextContent'
import { withRoutes } from '@hofs/withRoutes'
import { selectPageProps } from '@selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { selectUser, selectPagination } from '@selectors/context'
import { Pagination } from '@appTypes/pagination'
import { getSiteGroups } from '@services/getSiteGroups'

import { AdminGroupsTemplate } from '@components/_pageTemplates/AdminGroupsTemplate'
import { Props } from '@components/_pageTemplates/AdminGroupsTemplate/interfaces'

/**
 * Get props to inject into page on the initial server-side request
 */
 export const getServerSideProps: GetServerSideProps = async (context: GetServerSidePropsContext) => await pipeSSRProps(context, {
    routeId: '5943d34d-ee73-46da-bb9a-917ba8a2f421'
}, [
    withUser,
    withRoutes,
    withTextContent
], async (context: GetServerSidePropsContext) => {

    /**
     * Get data from request context
     */
    const props: Partial<Props> = selectPageProps(context);
    const user: User = selectUser(context)
    const pagination: Pagination = {
        pageNumber: selectPagination(context).pageNumber ?? 1,
        pageSize: selectPagination(context).pageSize ?? 20,
    }

    props.layoutId = layoutIds.ADMIN

    if (!props.actions.includes(actions.SITE_ADMIN_VIEW)) {
        return {
            notFound: true,
        }
    }

    /**
     * Get data from services
     */
    try {
        const [groupsList] = await Promise.all([
            getSiteGroups({ user, pagination }),
        ])

        props.groupsList = groupsList.data ?? []
        props.pagination = groupsList.pagination
    } catch (error) {
        return handleSSRErrorProps({ props, error })
    }

    /**
     * Return data to page template
     */
    return handleSSRSuccessProps({ props, context })

});

/**
 * Export page template
 */
export default AdminGroupsTemplate
