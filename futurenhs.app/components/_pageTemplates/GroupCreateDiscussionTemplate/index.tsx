import { useState } from 'react';
import { useRouter } from 'next/router';

import { selectFormDefaultFields, selectFormErrors, selectFormInitialValues } from '@selectors/forms';
import { formTypes } from '@constants/forms';
import { FormWithErrorSummary } from '@components/FormWithErrorSummary';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { postGroupDiscussion } from '@services/postGroupDiscussion';
import { FormErrors } from '@appTypes/form';

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
    const [errors, setErrors] = useState(selectFormErrors(forms, formTypes.CREATE_DISCUSSION));

    const fields = selectFormDefaultFields(forms, formTypes.CREATE_DISCUSSION);
    const initialValues = selectFormInitialValues(forms, formTypes.CREATE_DISCUSSION);

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

            const errors: FormErrors = {
                [error.data.status]: error.data.statusText
            };

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
                            formId={formTypes.CREATE_DISCUSSION}
                            fields={fields}
                            initialValues={initialValues}
                            errors={errors}
                            text={{
                                errorSummary: {
                                    body: 'There is a problem'
                                },
                                form: {
                                    submitButton: 'Create Discussion',
                                    cancelButton: 'Cancel'
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
