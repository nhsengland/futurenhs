import { GetServerSideProps } from 'next'

import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { layoutIds } from '@constants/routes'
import { actions } from '@constants/actions'
import { withUser } from '@hofs/withUser'
import { withTextContent } from '@hofs/withTextContent'
import { withRoutes } from '@hofs/withRoutes'
import { selectUser, selectPagination, selectPageProps } from '@selectors/context'
import { getSiteUsers } from '@services/getSiteUsers'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { Pagination } from '@appTypes/pagination'

import { AdminUsersTemplate } from '@components/_pageTemplates/AdminUsersTemplate'
import { Props } from '@components/_pageTemplates/AdminUsersTemplate/interfaces'

/**
 * Get props to inject into page on the initial server-side request
 */
 export const getServerSideProps: GetServerSideProps = async (context: GetServerSidePropsContext) => await pipeSSRProps(context, {
    routeId: '11fcd020-86e3-4935-982d-891bd86b52ff'
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
        const [usersList] = await Promise.all([
            getSiteUsers({ user, pagination }),
        ])

        props.usersList = usersList.data ?? []
        props.pagination = usersList.pagination
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
export default AdminUsersTemplate
