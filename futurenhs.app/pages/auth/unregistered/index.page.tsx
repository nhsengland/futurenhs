import { GetServerSideProps } from 'next'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { getSession } from 'next-auth/react'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { withRoutes } from '@hofs/withRoutes'
import { withTextContent } from '@hofs/withTextContent'
import { GetServerSidePropsContext } from '@appTypes/next'
import { selectPageProps } from '@selectors/context'
import { AuthUnregisteredTemplate } from '@components/_pageTemplates/AuthUnregisteredTemplate'
import { Props } from '@components/_pageTemplates/AuthUnregisteredTemplate/interfaces'

export const getServerSideProps: GetServerSideProps = async (
    context: GetServerSidePropsContext
) =>
    await pipeSSRProps(
        context,
        {
            routeId: '426b71e4-aa63-4386-a3ab-0769173f6456',
        },
        [withRoutes, withTextContent],
        async (context: GetServerSidePropsContext) => {
            const props: Partial<Props> = selectPageProps(context)
            const session = await getSession(context)

            /**
             * Hide breadcrumbs
             */
            ;(props as any).breadCrumbList = []

            /**
             * Redirect to site root if already signed out
             */
            if (!session) {
                return {
                    redirect: {
                        permanent: false,
                        destination: `${process.env.APP_URL}/auth/signin`,
                    },
                }
            }

            /**
             * Return data to page template
             */
            return handleSSRSuccessProps({ props, context })
        }
    )

/**
 * Export page template
 */
export default AuthUnregisteredTemplate
