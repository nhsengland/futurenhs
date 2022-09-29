import { GetServerSideProps } from 'next'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { getSession } from 'next-auth/react'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { withRoutes } from '@hofs/withRoutes'
import { withTextContent } from '@hofs/withTextContent'
import { GetServerSidePropsContext } from '@appTypes/next'
import { selectPageProps } from '@selectors/context'
import { Link } from '@components/Link'
import { PageBody } from '@components/PageBody'
import { RichText } from '@components/RichText'

import { Page } from '@appTypes/page'
import { GenericPageTextContent } from '@appTypes/content'

interface ContentText extends GenericPageTextContent {
    signOut: string
}

interface Props extends Page {
    contentText: ContentText
}

/**
 * Auth unregistered template
 */
const AuthUnregisteredPage: (props: Props) => JSX.Element = ({
    routes,
    contentText,
}) => {
    const { authSignOut } = routes
    const { mainHeading, bodyHtml, signOut } = contentText ?? {}

    return (
        <PageBody className="tablet:u-px-0">
            <h1 className="nhsuk-heading-xl">{mainHeading}</h1>
            <RichText
                wrapperElementType="div"
                className="u-mb-10"
                bodyHtml={bodyHtml}
            />
            {signOut && (
                <Link href={authSignOut}>
                    <a className="c-button c-button-outline u-drop-shadow">
                        {signOut}
                    </a>
                </Link>
            )}
        </PageBody>
    )
}

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

export default AuthUnregisteredPage
