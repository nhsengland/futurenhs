import { GetServerSideProps } from 'next'

import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { routeParams } from '@constants/routes'
import { selectUser, selectParam } from '@selectors/context'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withGroup } from '@hofs/withGroup'
import { getGroupFileDownload } from '@services/getGroupFileDownload'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'

import { Props } from '@components/_pageTemplates/GroupFileDetailTemplate/interfaces'

const NoopTemplate = (props: any) => null
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
            getServerSideProps: async (context: GetServerSidePropsContext) => {
                const user: User = selectUser(context)
                const groupId: string = selectParam(
                    context,
                    routeParams.GROUPID
                )
                const fileId: string = selectParam(context, routeParams.FILEID)

                /**
                 * Get data from services
                 */
                try {
                    const [groupFileDownloadLink] = await Promise.all([
                        getGroupFileDownload({ user, groupId, fileId }),
                    ])

                    return {
                        redirect: {
                            permanent: false,
                            destination: groupFileDownloadLink.data,
                        },
                    }
                } catch (error) {
                    return handleSSRErrorProps({ props, error })
                }
            },
        }),
    }),
})

/**
 * Export page template
 */
export default NoopTemplate
