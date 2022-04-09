import { useState } from 'react';
import { useRouter } from 'next/router';

import { formTypes } from '@constants/forms';
import { getServiceErrorDataValidationErrors } from '@services/index';
import { getGenericFormError } from '@helpers/util/form';
import { selectForm } from '@selectors/forms';
import { FormWithErrorSummary } from '@components/FormWithErrorSummary';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { RichText } from '@components/RichText';
import { postGroupFile } from '@services/postGroupFile';
import { FormErrors, FormConfig } from '@appTypes/form';

import { Props } from './interfaces';

/**
 * Group create file template
 */
export const GroupCreateFileTemplate: (props: Props) => JSX.Element = ({
    groupId,
    csrfToken,
    forms,
    routes,
    user,
    folderId,
    folder,
    contentText
}) => {

    const router = useRouter();

    const formConfig: FormConfig = selectForm(forms, formTypes.CREATE_FILE);
    const [errors, setErrors] = useState(formConfig?.errors);

    const { text } = folder ?? {};
    const { name } = text ?? {};
    const { secondaryHeading } = contentText ?? {};
    
    const folderHref: string = `${routes.groupFoldersRoot}/${folderId}`;

    const handleSubmit = async (formData: FormData): Promise<FormErrors> => {

        return new Promise((resolve) => {

            postGroupFile({ groupId, folderId, user, body: formData }).then(() => {

                router.push(folderHref);
                resolve({});

            })
            .catch((error) => {

                const errors: FormErrors = getServiceErrorDataValidationErrors(error) || getGenericFormError(error);
    
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
                            formConfig={formConfig}
                            errors={errors}
                            text={{
                                errorSummary: {
                                    body: 'There is a problem'
                                },
                                form: {
                                    submitButton: 'Upload File',
                                    cancelButton: 'Discard File'
                                }
                            }}
                            submitAction={handleSubmit}
                            cancelHref={folderHref}
                            bodyClassName=""
                            submitButtonClassName="u-float-right">
                                <h2 className="nhsuk-heading-l u-mb-6 o-truncated-text-lines-1">{name}</h2>
                                <hr />
                                <h3 className="u-mt-6">{secondaryHeading}</h3>
                                <RichText wrapperElementType="div" className="u-mb-10" bodyHtml="<p>Guidance on maximum file size, supported file formats, making sure they are not uploading any sensitive data, etc.</p><p>All uploaded content must conform to the platform's terms and conditions. 
Open terms and conditions in a new window.</p>" />
                        </FormWithErrorSummary>
                    </LayoutColumn>
                </LayoutColumnContainer>
            </LayoutColumn>
        </>

    )

}
