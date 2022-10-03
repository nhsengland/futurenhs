import { GetServerSideProps } from 'next'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { routeParams } from '@constants/routes'
import { withUser } from '@helpers/hofs/withUser'
import { withTextContent } from '@helpers/hofs/withTextContent'
import { withRoutes } from '@helpers/hofs/withRoutes'
import { selectParam, selectUser } from '@helpers/selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { getSiteUser } from '@services/getSiteUser'
import { selectPageProps } from '@helpers/selectors/context'
import { User } from '@appTypes/user'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { UserProfile } from '@components/generic/UserProfile'
import { actions as actionsConstants } from '@constants/actions'
import { Link } from '@components/generic/Link'
import { GenericPageTextContent } from '@appTypes/content'
import { Page } from '@appTypes/page'

interface ContentText extends GenericPageTextContent {
    firstNameLabel: string
    lastNameLabel: string
    pronounsLabel: string
    emailLabel: string
    editHeading?: string
    editButtonLabel?: string
}

export interface Props extends Page {
    siteUser: any
    contentText: ContentText
}

/**
 * Site user template
 */
export const UserPage: (props: Props) => JSX.Element = ({
    contentText,
    siteUser,
    actions,
    user,
    routes,
}) => {
    const {
        secondaryHeading,
        firstNameLabel,
        lastNameLabel,
        pronounsLabel,
        emailLabel,
        editButtonLabel,
    } = contentText ?? {}

    const hasEditPermissions: boolean =
        actions.includes(actionsConstants.SITE_ADMIN_MEMBERS_EDIT) ||
        siteUser.id === user.id
    const shouldRenderEditButton: boolean = hasEditPermissions

    /**
     * Render
     */
    return (
        <LayoutColumn className="c-page-body">
            <LayoutColumnContainer>
                <LayoutColumn tablet={6}>
                    <UserProfile
                        headingLevel={1}
                        profile={siteUser}
                        image={siteUser.image}
                        text={{
                            heading: secondaryHeading,
                            firstNameLabel: firstNameLabel,
                            lastNameLabel: lastNameLabel,
                            pronounsLabel: pronounsLabel,
                            emailLabel: emailLabel,
                        }}
                        className="tablet:u-justify-center tablet:u-mt-16"
                    >
                        {shouldRenderEditButton && (
                            <Link
                                href={`${routes.usersRoot}/${siteUser.id}/update`}
                            >
                                <a className="c-form_submit-button c-button u-w-full tablet:u-w-auto u-mt-8 c-button-outline">
                                    {editButtonLabel}
                                </a>
                            </Link>
                        )}
                    </UserProfile>
                </LayoutColumn>
            </LayoutColumnContainer>
        </LayoutColumn>
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
            routeId: '9e86c5cc-6836-4319-8d9d-b96249d4c909',
        },
        [withUser, withRoutes, withTextContent],
        async (context: GetServerSidePropsContext) => {
            /**
             * Get data from request context
             */
            const props: Partial<Props> = selectPageProps(context)
            const userId: string = selectParam(context, routeParams.USERID)
            const user: User = selectUser(context)

            /**
             * Get data from services
             */
            try {
                const [userData] = await Promise.all([
                    getSiteUser({ user, targetUserId: userId }),
                ])

                props.siteUser = userData.data
                props.pageTitle = `${props.contentText.title} - ${
                    props.siteUser.firstName ?? ''
                } ${props.siteUser.lastName ?? ''}`
            } catch (error: any) {
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
export default UserPage
