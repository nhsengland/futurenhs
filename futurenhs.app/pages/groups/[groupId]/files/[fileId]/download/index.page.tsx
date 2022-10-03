import { GetServerSideProps } from 'next'

import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { routeParams } from '@constants/routes'
import { selectUser, selectParam } from '@helpers/selectors/context'
import { withUser } from '@helpers/hofs/withUser'
import { withRoutes } from '@helpers/hofs/withRoutes'
import { withGroup } from '@helpers/hofs/withGroup'
import { selectPageProps } from '@helpers/selectors/context'
import { getGroupFileDownload } from '@services/getGroupFileDownload'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'

const NoopTemplate = (props: any) => null

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (
    context: GetServerSidePropsContext
) =>
    await pipeSSRProps(
        context,
        {},
        [withUser, withRoutes, withGroup],
        async (context: GetServerSidePropsContext) => {
            /**
             * Get data from request context
             */
            const props: Partial<any> = selectPageProps(context)
            const user: User = selectUser(context)
            const groupId: string = selectParam(context, routeParams.GROUPID)
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
        }
    )

/**
 * Export page template
 */
export default NoopTemplate
