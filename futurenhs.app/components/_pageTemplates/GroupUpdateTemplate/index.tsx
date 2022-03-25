import { useState } from 'react';
import { useRouter } from 'next/router';

import { formTypes } from "@constants/forms";
import { getStandardServiceHeaders } from '@helpers/fetch';
import { selectFormErrors, selectFormInitialValues, selectFormDefaultFields } from '@selectors/forms';
import { FormWithErrorSummary } from '@components/FormWithErrorSummary';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { putGroup } from '@services/putGroup';
import { FormErrors } from '@appTypes/form';

import { Props } from './interfaces';

/**
 * Group create folder template
 */
export const GroupUpdateTemplate: (props: Props) => JSX.Element = ({
    groupId,
    user,
    csrfToken,
    etag,
    forms,
    routes,
    contentText,
    services = {
        putGroup: putGroup
    }
}) => {

    const router = useRouter();

    const [errors, setErrors] = useState(selectFormErrors(forms, formTypes.UPDATE_GROUP));
    const [initialValues] = useState(selectFormInitialValues(forms, formTypes.UPDATE_GROUP));

    const fields = selectFormDefaultFields(forms, formTypes.UPDATE_GROUP);

    const { mainHeading, secondaryHeading } = contentText ?? {};
    
    /**
     * Handle client-side update submission
     */
    const handleSubmit = async (formData: FormData): Promise<FormErrors> => {

        /**
         * NB - clearing the existing image is currently supported client-side only
         */
        const shouldClearGroupImage: boolean = Boolean(formData.get('File-clear'));

        if(shouldClearGroupImage){

            formData.set('ImageId', '');

        }

        return new Promise((resolve) => {

            const headers = getStandardServiceHeaders({ csrfToken, etag });

            services.putGroup({ groupId, user, headers, body: formData }).then(() => {

                setErrors({});
                resolve({});

                router.replace(routes.groupRoot);

                /**
                 * Full page reload currently necessary to clear image cache of previous group image
                 */
                window.location.replace(routes.groupRoot)

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
                                    cancelButton: 'Discard changes'
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
