import { useState } from 'react';
import { useRouter } from 'next/router';

import { getServiceErrorDataValidationErrors } from '@services/index';
import { getGenericFormError } from '@helpers/util/form';
import { selectForm } from '@selectors/forms';
import { formTypes } from '@constants/forms';
import { FormWithErrorSummary } from '@components/FormWithErrorSummary';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { postGroupDiscussion } from '@services/postGroupDiscussion';
import { FormErrors, FormConfig } from '@appTypes/form';

import { Props } from './interfaces';

/**
 * Group create discussion template
 */
export const GroupCreateDiscussionTemplate: (props: Props) => JSX.Element = ({
    groupId,
    csrfToken,
    forms,
    routes,
    user,
    contentText,
    services = {
        postGroupDiscussion: postGroupDiscussion
    }
}) => {

    const router = useRouter();

    const formConfig: FormConfig = selectForm(forms, formTypes.CREATE_DISCUSSION);
    const [errors, setErrors] = useState(formConfig?.errors);

    const { secondaryHeading } = contentText ?? {};

    /**
     * Client-side submission handler
     */
    const handleSubmit = async (formData: FormData): Promise<FormErrors> => {

        try {

            await services.postGroupDiscussion({ groupId, user, body: formData as any });

            router.push(routes.groupForumRoot);

            return Promise.resolve({});

        } catch (error) {

            const errors: FormErrors = getServiceErrorDataValidationErrors(error) || getGenericFormError(error);

            setErrors(errors);

            return Promise.resolve(errors);

        }

    };

    /**
     * Render
     */
    return (

        <>
            <LayoutColumn className="c-page-body">
                <LayoutColumnContainer>
                    <LayoutColumn tablet={8}>
                        <FormWithErrorSummary
                            csrfToken={csrfToken}
                            formConfig={formConfig}
                            errors={errors}
                            text={{
                                errorSummary: {
                                    body: 'There is a problem'
                                },
                                form: {
                                    submitButton: 'Create Discussion',
                                    cancelButton: 'Discard Discussion'
                                }
                            }}
                            submitAction={handleSubmit}
                            cancelHref={routes.groupForumRoot}
                            bodyClassName="u-mb-14 u-p-4 tablet:u-px-14 tablet:u-pt-12 u-pb-8 u-bg-theme-1">
                                <h2 className="nhsuk-heading-l">{secondaryHeading}</h2>
                        </FormWithErrorSummary>
                    </LayoutColumn>
                </LayoutColumnContainer>
            </LayoutColumn>
        </>

    )

}
