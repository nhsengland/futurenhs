import { useState } from 'react';
import { useRouter } from 'next/router';

import { formTypes } from '@constants/forms';
import { getStandardServiceHeaders } from '@helpers/fetch';
import { selectFormDefaultFields, selectFormInitialValues, selectFormErrors } from '@selectors/forms';
import { FormWithErrorSummary } from '@components/FormWithErrorSummary';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { RichText } from '@components/RichText';
import { postGroupFolder } from '@services/postGroupFolder';
import { putGroupFolder } from '@services/putGroupFolder';
import { FormErrors } from '@appTypes/form';

import { Props } from './interfaces';

/**
 * Group create/update folder template
 */
export const GroupCreateUpdateFolderTemplate: (props: Props) => JSX.Element = ({
    etag,
    groupId,
    folderId,
    csrfToken,
    forms,
    routes,
    user,
    folder,
    contentText
}) => {

    const router = useRouter();
    const [errors, setErrors] = useState(selectFormErrors(forms, formTypes.GROUP_FOLDER));

    const initialValues = selectFormInitialValues(forms, formTypes.GROUP_FOLDER);
    const fields = selectFormDefaultFields(forms, formTypes.GROUP_FOLDER);

    const { text } = folder ?? {};
    const { name } = text ?? {};
    const {secondaryHeading} = contentText ?? {};

    const handleSubmit = async (formData: FormData): Promise<FormErrors> => {

        return new Promise((resolve) => {

            const serviceToUse = router.asPath.indexOf('/update') > -1 ? putGroupFolder : postGroupFolder;
            const headers = getStandardServiceHeaders({ csrfToken, etag });

            serviceToUse({ groupId, folderId, user, headers, body: formData }).then(() => {

                router.push(routes.groupFolder || routes.groupFoldersRoot);

                resolve({});

            })
            .catch((error) => {

                const errors: FormErrors = error.data ? {
                    [error.data.status]: error.data.statusText
                } : {
                    ['error']: error.message
                };
    
                setErrors(errors);
                resolve(errors);

            });

        });

    }

    return (

        <>
            <LayoutColumn className="c-page-body">
                <LayoutColumnContainer>
                    <LayoutColumn tablet={8}>
                        <FormWithErrorSummary
                            csrfToken={csrfToken}
                            formId={formTypes.GROUP_FOLDER}
                            fields={fields}
                            errors={errors}
                            initialValues={initialValues}
                            text={{
                                errorSummary: {
                                    body: 'There is a problem'
                                },
                                form: {
                                    submitButton: 'Save and continue',
                                    cancelButton: 'Discard folder'
                                }
                            }}
                            submitAction={handleSubmit}
                            cancelHref={routes.groupFolder || routes.groupFoldersRoot}
                            bodyClassName="u-mb-14 u-p-4 tablet:u-px-14 tablet:u-pt-12 u-pb-8 u-bg-theme-1"
                            submitButtonClassName="u-float-right">
                                {name &&
                                    <>
                                        <h2 className="nhsuk-heading-l o-truncated-text-lines-3">{name}</h2>
                                        <hr />
                                    </>
                                }
                                {name ? <h3 className="nhsuk-heading-m">{secondaryHeading}</h3> : <h2 className="nhsuk-heading-l">{secondaryHeading}</h2>}
                        </FormWithErrorSummary>
                    </LayoutColumn>
                </LayoutColumnContainer>
            </LayoutColumn>
        </>

    )

}
