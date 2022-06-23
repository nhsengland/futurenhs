import { GetServerSideProps } from 'next'

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'

import { actions as actionConstants } from '@constants/actions'
import { groupTabIds, layoutIds } from '@constants/routes'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withTokens } from '@hofs/withTokens'

import { GetServerSidePropsContext } from '@appTypes/next'

import { GroupMemberInviteTemplate } from '@components/_pageTemplates/GroupMemberInviteTemplate'
import { Props } from '@components/_pageTemplates/GroupCreateDiscussionTemplate/interfaces'
import { withTextContent } from '@hofs/withTextContent'
import { withGroup } from '@hofs/withGroup'

const routeId: string = 'f872b71a-0449-4821-a8da-b75bbd451b2d'
const props: Partial<Props> = {}

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withUser({
    props,
    getServerSideProps: withRoutes({
        props,
        getServerSideProps: withGroup({
            props,
            getServerSideProps: withTokens({
                props,
                routeId,
                getServerSideProps: withTextContent({
                    props,
                    routeId,
                    getServerSideProps: async (
                        context: GetServerSidePropsContext
                    ) => {
                        props.layoutId = layoutIds.GROUP
                        props.tabId = groupTabIds.MEMBERS
                        props.pageTitle = `${props.entityText.title} - ${props.contentText.subTitle}`

                        /**
                         * Return page not found if user doesn't have permissions to invite a user - TODO: Pending API
                         */
                        // if (
                        //     !props.actions?.includes(
                        //         actionConstants.GROUPS_MEMBERS_INVITE
                        //     )
                        // ) {
                        //     return {
                        //         notFound: true,
                        //     }
                        // }

                        /**
                         * Return data to page template
                         */
                        return handleSSRSuccessProps({ props })
                    },
                }),
            }),
        }),
    }),
})

/**
 * Export page template
 */
export default GroupMemberInviteTemplate
