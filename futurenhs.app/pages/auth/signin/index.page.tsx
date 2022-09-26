import { GetServerSideProps } from 'next'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { getSession } from 'next-auth/react'
import { getAuthCsrfData } from '@services/getAuthCsrfData'
import { selectQuery } from '@selectors/context'
import { handleSSRAuthSuccessProps } from '@helpers/util/ssr/handleSSRAuthSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { withRoutes } from '@hofs/withRoutes'
import { withTextContent } from '@hofs/withTextContent'
import { selectPageProps } from '@selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'

import { AuthSignInTemplate } from '@components/_pageTemplates/AuthSignInTemplate'
import { Props } from '@components/_pageTemplates/AuthSignInTemplate/interfaces'

export const getServerSideProps: GetServerSideProps = async (
    context: GetServerSidePropsContext
) =>
    await pipeSSRProps(
        context,
        {
            routeId: '46524db4-44eb-4296-964d-69dfc2279f01',
        },
        [withRoutes, withTextContent],
        async (context: GetServerSidePropsContext) => {
            const props: Partial<Props> = selectPageProps(context)
            const { query } = context
            const error: string = selectQuery(context, 'error')
            const session = await getSession(context)

            /**
             * Set base template properties
             */
            ;(props as any).breadCrumbList = []
            ;(props as any).shouldRenderMainNav = false
            ;(props as any).className = 'u-bg-theme-3'

            /**
             * Redirect to site root if already signed in
             */
            if (session) {
                return {
                    redirect: {
                        permanent: false,
                        destination: `${process.env.APP_URL}/groups`,
                    },
                }
            }

            /**
             * Handle any returned OAuth errors
             */
            if (error) {
                props.errors = [
                    {
                        [500]: error,
                    },
                ]

                return handleSSRAuthSuccessProps({ props, context })
            }

            /**
             * Get data from services
             */
            try {
                /**
                 * Get next-auth specific csrf token and associated cookie header
                 */
                const [csrfData] = await Promise.all([
                    getAuthCsrfData({ query }),
                ])
                props.csrfToken = csrfData.data

                /**
                 * next-auth assumes a browser -> server request, so the returned set-cookie header needs
                 * passing through in order for the csrf token validation to succeed on form POST
                 */
                context.res.setHeader(
                    'Set-Cookie',
                    csrfData.headers.get('Set-Cookie')
                )
            } catch (error) {
                return handleSSRErrorProps({ props, error })
            }

            /**
             * Return data to page template
             */
            return handleSSRAuthSuccessProps({ props, context })
        }
    )

/**
 * Export page template
 */
export default AuthSignInTemplate
