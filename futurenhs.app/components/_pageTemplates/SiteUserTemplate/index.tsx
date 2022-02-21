import { useRouter } from 'next/router';

import { routeParams } from '@constants/routes';
import { StandardLayout } from '@components/_pageLayouts/StandardLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { BackLink } from '@components/BackLink';
import { UserProfile } from '@components/UserProfile';
import { getRouteToParam } from '@helpers/routing/getRouteToParam';

import { Props } from './interfaces';  

/**
 * Site user template
 */
export const SiteUserTemplate: (props: Props) => JSX.Element = ({
    user,
    entityText,
    contentText,
    member,
    actions,
    theme,
    image
}) => {

    const { secondaryHeading, 
            firstNameLabel, 
            lastNameLabel, 
            pronounsLabel, 
            emailLabel } = contentText ?? {};

    const backLinkHref: string = '/';

    return (

        <StandardLayout 
            user={user}
            actions={actions}
            className="u-bg-theme-3">
                <LayoutColumn className="c-page-body">
                    <BackLink 
                        href={backLinkHref}
                        text={{
                            link: "Back"
                        }} />
                    <LayoutColumnContainer justify="centre">
                        <LayoutColumn tablet={11}>
                            <UserProfile
                                member={member}
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
        </StandardLayout>

    )

}