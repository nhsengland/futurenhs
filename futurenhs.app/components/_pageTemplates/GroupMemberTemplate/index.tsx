import { useRouter } from 'next/router';

import { formTypes } from '@constants/forms';
import { actions as actionsConstants } from '@constants/actions';
import { routeParams } from '@constants/routes';
import { selectFormDefaultFields } from '@selectors/forms';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { BackLink } from '@components/BackLink';
import { UserProfile } from '@components/UserProfile';
import { Form } from '@components/Form';
import { ErrorSummary } from '@components/ErrorSummary';
import { Accordion } from '@components/Accordion';
import { getRouteToParam } from '@helpers/routing/getRouteToParam';
import { FormErrors } from '@appTypes/form';

import { Props } from './interfaces';

/**
 * Group member template
 */
export const GroupMemberTemplate: (props: Props) => JSX.Element = ({
    contentText,
    member,
    forms,
    actions,
}) => {

    const router = useRouter();

    const fields = selectFormDefaultFields(forms, formTypes.UPDATE_GROUP_MEMBER);

    const { secondaryHeading,
            firstNameLabel,
            lastNameLabel,
            pronounsLabel,
            emailLabel } = contentText ?? {};

    const backLinkHref: string = getRouteToParam({
        router: router,
        paramName: routeParams.MEMBERID
    });

    const shouldRenderUpdateForm: boolean = actions.includes(actionsConstants.GROUPS_MEMBERS_EDIT);
    const shouldRenderAccordionOpen: boolean = Boolean(router.query.edit);

    /**
     * Handle client-side update submission
     */
     const handleSubmit = async (formData: FormData): Promise<FormErrors> => {

        return new Promise((resolve) => {

            // TODO
            resolve({});

        });

    };

    return (

        <LayoutColumn className="c-page-body">
            <BackLink
                href={backLinkHref}
                text={{
                    link: "Back"
                }} />
            <LayoutColumnContainer justify="centre">
                <LayoutColumn tablet={11}>
                    {shouldRenderUpdateForm &&
                        <ErrorSummary />
                    }
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
                    {shouldRenderUpdateForm &&
                        <Accordion 
                            id=""
                            isOpen={shouldRenderAccordionOpen}
                            toggleChildren="Edit member">
                                <Form
                                    csrfToken=""
                                    formId=""
                                    fields={fields}
                                    text={{
                                        submitButton: 'Save Changes'
                                    }}
                                    submitAction={handleSubmit} />
                        </Accordion>
                    }
                </LayoutColumn>
            </LayoutColumnContainer>
        </LayoutColumn>

    )

}