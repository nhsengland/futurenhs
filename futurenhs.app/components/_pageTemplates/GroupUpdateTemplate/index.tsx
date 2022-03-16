import { useState } from 'react';
import { formTypes } from "@constants/forms";
import { selectFormErrors, selectFormInitialValues, selectFormDefaultFields } from '@selectors/forms';
import { FormWithErrorSummary } from '@components/FormWithErrorSummary';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { putGroupDetails } from '@services/putGroupDetails';
import { FormErrors } from '@appTypes/form';

import { Props } from './interfaces';
import { routes } from '@jestMocks/generic-props';

/**
 * Group create folder template
 */
export const GroupUpdateTemplate: (props: Props) => JSX.Element = ({
    groupId,
    user,
    csrfToken,
    forms,
    contentText,
    services = {
        putGroupDetails: putGroupDetails
    }
}) => {

    const [errors, setErrors] = useState(selectFormErrors(forms, formTypes.UPDATE_GROUP));

    const initialValues = selectFormInitialValues(forms, formTypes.UPDATE_GROUP);
    const fields = selectFormDefaultFields(forms, formTypes.UPDATE_GROUP);

    const { mainHeading, secondaryHeading } = contentText ?? {};
    
    /**
     * Handle client-side update submission
     */
    const handleSubmit = async (formData: FormData): Promise<FormErrors> => {

        return new Promise((resolve) => {

            services.putGroupDetails({ groupId, user, csrfToken, body: formData }).then(() => {

                setErrors({});
                resolve({});

            })
            .catch((error) => {

                const errors: FormErrors = {
                    [error.data.status]: error.data.statusText
                };

                setErrors(errors);
                resolve(errors);

            });

        });

    };

    return (

        <>
            <LayoutColumn className="c-page-body">
                <LayoutColumnContainer>
                    <LayoutColumn tablet={8}>
                        <FormWithErrorSummary
                            csrfToken={csrfToken}
                            formId={formTypes.UPDATE_GROUP}
                            fields={fields}
                            errors={errors}
                            initialValues={initialValues}
                            text={{
                                errorSummary: {
                                    body: 'There was a problem'
                                },
                                form: {
                                    submitButton: 'Save and close',
                                    cancelButton: 'Cancel'
                                }
                            }}
                            cancelHref={routes.groupRoot}
                            submitAction={handleSubmit}
                            bodyClassName="u-mb-12">
                                <h2 className="nhsuk-heading-l">{mainHeading}</h2>
                                <p className="u-text-lead u-text-theme-7 u-mb-4">{secondaryHeading}</p>
                        </FormWithErrorSummary>
                    </LayoutColumn>
                </LayoutColumnContainer>
            </LayoutColumn>
        </>

    )

}
