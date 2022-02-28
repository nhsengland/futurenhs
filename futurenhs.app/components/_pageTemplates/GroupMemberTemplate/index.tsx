import { useRouter } from 'next/router';

import { formTypes } from '@constants/forms';
import { actions as actionsConstants } from '@constants/actions';
import { routeParams } from '@constants/routes';
import { selectFormDefaultFields } from '@selectors/forms';
import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { BackLink } from '@components/BackLink';
import { UserProfile } from '@components/UserProfile';
import { FormWithErrorSummary } from '@components/FormWithErrorSummary';
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
    forms,
    actions,
    themeId,
    image
}) => {

    const fields = selectFormDefaultFields(forms, formTypes.UPDATE_GROUP_MEMBER);

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

    const shouldRenderUpdateForm: boolean = actions.includes(actionsConstants.GROUPS_MEMBERS_EDIT);

    const renderUserProfile = () => {
        
        return (
        
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

        )

    };

    return (

        <GroupLayout 
            tabId="members"
            themeId={themeId}
            user={user}
            actions={actions}
            text={entityText}
            image={image} 
            className="u-bg-theme-3">
                <LayoutColumn className="c-page-body">
                    <BackLink 
                        href={backLinkHref}
                        text={{
                            link: "Back"
                        }} />
                    <LayoutColumnContainer justify="centre">
                        <LayoutColumn tablet={11}>
                            {shouldRenderUpdateForm
                            
                                ?   <FormWithErrorSummary 
                                        csrfToken=""
                                        formId=""
                                        fields={fields}
                                        errors={{}}
                                        text={{
                                            errorSummary: {
                                                body: 'There was a problem'
                                            },
                                            form: {
                                                submitButton: 'Save changes'
                                            }
                                        }}
                                        submitAction={() => {}}>
                                            {renderUserProfile()}
                                    </FormWithErrorSummary>

                                :   renderUserProfile()
                                
                            }  
                        </LayoutColumn>
                    </LayoutColumnContainer>
                </LayoutColumn>
        </GroupLayout>

    )

}