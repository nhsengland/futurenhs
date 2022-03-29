import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { BackLink } from '@components/BackLink';
import { UserProfile } from '@components/UserProfile';

import { Props } from './interfaces';

/**
 * Site user template
 */
export const SiteUserTemplate: (props: Props) => JSX.Element = ({
    contentText,
    siteUser,
    routes
}) => {

    const { secondaryHeading,
            firstNameLabel,
            lastNameLabel,
            pronounsLabel,
            emailLabel } = contentText ?? {};

    return (

        <LayoutColumn className="c-page-body">
            <BackLink
                href={routes.siteRoot}
                text={{
                    link: "Back"
                }} />
            <LayoutColumnContainer justify="centre">
                <LayoutColumn tablet={11}>
                    <UserProfile
                        profile={siteUser}
                        text={{
                            heading: secondaryHeading,
                            firstNameLabel: firstNameLabel,
                            lastNameLabel: lastNameLabel,
                            pronounsLabel: pronounsLabel,
                            emailLabel: emailLabel
                        }}
                        className="tablet:u-justify-center tablet:u-mt-16" />
                </LayoutColumn>
            </LayoutColumnContainer>
        </LayoutColumn>

    )

}