import { GetServerSideProps } from 'next'

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { layoutIds, groupTabIds, routeParams } from '@constants/routes'
import { withUser } from '@hofs/withUser'
import { withGroup } from '@hofs/withGroup'
import { withRoutes } from '@hofs/withRoutes'
import { withTextContent } from '@hofs/withTextContent'
import { User } from '@appTypes/user'
import { selectUser, selectParam } from '@selectors/context'
import { getGroupHomePageContentBlocks } from '@services/getGroupHomePageContentBlocks'
import { GetServerSidePropsContext } from '@appTypes/next'

import { GroupHomeTemplate } from '@components/_pageTemplates/GroupHomeTemplate'
import { Props } from '@components/_pageTemplates/GroupHomeTemplate/interfaces'

const routeId: string = '7a9bdd18-45ea-4976-9810-2fcb66242e27'
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
            routeId,
            getServerSideProps: withTextContent({
                props,
                routeId,
                getServerSideProps: async (
                    context: GetServerSidePropsContext
                ) => {

                    const user: User = selectUser(context)
                    const groupId: string = selectParam(context, routeParams.GROUPID)

                    props.layoutId = layoutIds.GROUP
                    props.tabId = groupTabIds.INDEX
                    props.pageTitle = props.entityText.title

                    /**
                     * Get data from services
                     */
                    try {
                        const [contentBlocks] =
                            await Promise.all([
                                getGroupHomePageContentBlocks({ user, groupId })
                            ])

                        props.contentBlocks = contentBlocks.data

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
    }),
})

/**
 * Export page template
 */
export default GroupHomeTemplate
