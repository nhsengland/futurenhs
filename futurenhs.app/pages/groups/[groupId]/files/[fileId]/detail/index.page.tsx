import { GetServerSideProps } from 'next'

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { layoutIds, groupTabIds } from '@constants/routes'
import { routeParams } from '@constants/routes'
import { selectUser, selectParam } from '@selectors/context'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withGroup } from '@hofs/withGroup'
import { withTextContent } from '@hofs/withTextContent'
import { getGroupFile } from '@services/getGroupFile'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'

import { GroupFileDetailTemplate } from '@components/_pageTemplates/GroupFileDetailTemplate'
import { Props } from '@components/_pageTemplates/GroupFileDetailTemplate/interfaces'

const routeId: string = 'b74b9b6b-0462-4c2a-8859-51d0df17f68f'
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
            getServerSideProps: withTextContent({
                props,
                routeId,
                getServerSideProps: async (
                    context: GetServerSidePropsContext
                ) => {
                    const user: User = selectUser(context)
                    const groupId: string = selectParam(
                        context,
                        routeParams.GROUPID
                    )
                    const fileId: string = selectParam(
                        context,
                        routeParams.FILEID
                    )

                    props.fileId = fileId
                    props.layoutId = layoutIds.GROUP
                    props.tabId = groupTabIds.FILES

                    /**
                     * Get data from services
                     */
                    try {
                        const [groupFile] = await Promise.all([
                            getGroupFile({ user, groupId, fileId }),
                        ])

                        props.file = groupFile.data
                        props.pageTitle = `${props.entityText.title} - ${props.file.name}`
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
export default GroupFileDetailTemplate
