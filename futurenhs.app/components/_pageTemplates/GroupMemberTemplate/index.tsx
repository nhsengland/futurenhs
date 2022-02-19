import { useRouter } from 'next/router';

import { routeParams } from '@constants/routes';
import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { BackLink } from '@components/BackLink';
import { UserProfile } from '@components/UserProfile';
import { getRouteToParam } from '@helpers/routing/getRouteToParam';

import { Props } from './interfaces';  

/**
 * Group member template
 */
export const GroupMemberTemplate: (props: Props) => JSX.Element = ({
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

    const router = useRouter();
    const backLinkHref: string = getRouteToParam({
        router: router,
        paramName: routeParams.MEMBERID
    });

    return (

        <GroupLayout 
            id="members"
            user={user}
            actions={actions}
            text={entityText}
            image={image} 
            theme={theme}
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
        </GroupLayout>

    )

}