
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { UserProfile } from '@components/UserProfile'
import { actions as actionsConstants } from '@constants/actions'
import { Link } from '@components/Link'

import { Props } from './interfaces'


/**
 * Site user template
 */
export const SiteUserTemplate: (props: Props) => JSX.Element = ({
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
        editButtonLabel
    } = contentText ?? {}

    const hasEditPermissions: boolean = actions.includes(actionsConstants.SITE_ADMIN_MEMBERS_EDIT) || siteUser.id === user.id;
    const shouldRenderEditButton: boolean = hasEditPermissions;


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
                                {shouldRenderEditButton &&
                                    <Link href={`${routes.usersRoot}/${siteUser.id}/update`}><a className="c-form_submit-button c-button u-w-full tablet:u-w-auto u-mt-8 c-button-outline">{editButtonLabel}</a></Link>
                                }
                            </UserProfile>
                    </LayoutColumn>
                </LayoutColumnContainer>
            </LayoutColumn>
    )
}
