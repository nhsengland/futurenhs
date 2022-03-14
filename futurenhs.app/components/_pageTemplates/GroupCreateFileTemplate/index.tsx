import { useState } from 'react';
import { useRouter } from 'next/router';

import { formTypes } from '@constants/forms';
import { selectFormDefaultFields, selectFormInitialValues, selectFormErrors } from '@selectors/forms';
import { FormWithErrorSummary } from '@components/FormWithErrorSummary';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { RichText } from '@components/RichText';
import { postGroupFile } from '@services/postGroupFile';
import { FormErrors } from '@appTypes/form';

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
    const [errors, setErrors] = useState(selectFormErrors(forms, formTypes.CREATE_FILE));

    const initialValues = selectFormInitialValues(forms, formTypes.CREATE_FILE);
    const fields = selectFormDefaultFields(forms, formTypes.CREATE_FILE);


    const { text } = folder ?? {};
    const { name } = text ?? {};
    const { secondaryHeading } = contentText ?? {};
    
    const folderHref: string = `${routes.groupFoldersRoot}/${folderId}`;

    const handleSubmit = async (formData: FormData): Promise<FormErrors> => {

        return new Promise((resolve) => {

            postGroupFile({ groupId, folderId, user, csrfToken, body: formData }).then(() => {

                router.push(folderHref);
                resolve({});

            })
            .catch((error) => {

                if(error.data){

                    setErrors({
                        [error.data.status]: error.data.statusText
                    });
    
                } else {
    
                    setErrors({
                        ['Error']: error.message
                    });
    
                }
    
                resolve({});

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
                            formId={formTypes.CREATE_FILE}
                            fields={fields}
                            errors={errors}
                            initialValues={initialValues}
                            text={{
                                errorSummary: {
                                    body: 'There is a problem'
                                },
                                form: {
                                    submitButton: 'Upload and continue',
                                    cancelButton: 'Cancel'
                                }
                            }}
                            submitAction={handleSubmit}
                            cancelHref={folderHref}
                            bodyClassName=""
                            submitButtonClassName="u-float-right">
                                <h2 className="nhsuk-heading-l u-mb-6">{name}</h2>
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
