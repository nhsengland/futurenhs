import { GetServerSideProps } from 'next'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { layoutIds } from '@constants/routes'
import { actions } from '@constants/actions'
import { withUser } from '@helpers/hofs/withUser'
import { withTextContent } from '@helpers/hofs/withTextContent'
import { withRoutes } from '@helpers/hofs/withRoutes'
import { selectUser, selectPageProps } from '@helpers/selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { Page } from '@appTypes/page'
import { GenericPageTextContent } from '@appTypes/content'

declare interface ContentText extends GenericPageTextContent {
    mainHeading: string
}

export interface Props extends Page {
    contentText: ContentText
}

/**
 * Admin users dashboard template
 */
export const AdminDomainsPage: (props: Props) => JSX.Element = ({
    contentText,
    actions,
    routes,
    user,
    csrfToken,
}) => {
    const { secondaryHeading } = contentText ?? {}

    return (
        <>
            <LayoutColumnContainer className="u-w-full u-flex u-flex-col tablet:u-flex-row">
                <LayoutColumn tablet={9} className="c-page-body">
                    <h2 className="nhsuk-heading-l">{secondaryHeading}</h2>
                </LayoutColumn>
            </LayoutColumnContainer>
        </>
    )
}

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (
    context: GetServerSidePropsContext
) =>
    await pipeSSRProps(
        context,
        {
            routeId: 'd7016549-cb04-457e-b5d8-caa49e408ecb',
        },
        [withUser, withRoutes, withTextContent],
        async (context: GetServerSidePropsContext) => {
            /**
             * Get data from request context
             */
            const props: Partial<Props> = selectPageProps(context)
            const user: User = selectUser(context)

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
            } catch (error) {
                return handleSSRErrorProps({ props, error })
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
export default AdminDomainsPage
