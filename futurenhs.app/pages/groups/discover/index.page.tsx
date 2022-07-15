import { GetServerSideProps } from 'next'

import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withTextContent } from '@hofs/withTextContent'
import { getGroups } from '@services/getGroups'
import { selectUser, selectPagination, selectPageProps } from '@selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { Pagination } from '@appTypes/pagination'

import { GroupListingTemplate } from '@components/_pageTemplates/GroupListingTemplate'
import { Props } from '@components/_pageTemplates/GroupListingTemplate/interfaces'

const isMember: boolean = false

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (context: GetServerSidePropsContext) => await pipeSSRProps(context, {
    routeId: '8190d347-e29a-4577-baa8-446bcae428d9'
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
        pageSize: selectPagination(context).pageSize ?? 10,
    }

    props.isGroupMember = isMember

    /**
     * Get data from services
     */
    try {
        const [groupsList] = await Promise.all([
            getGroups({ user, isMember, pagination }),
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
export default GroupListingTemplate
