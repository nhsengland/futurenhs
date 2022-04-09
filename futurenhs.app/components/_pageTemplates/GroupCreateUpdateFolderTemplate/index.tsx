import { useState } from 'react';
import { useRouter } from 'next/router';

import { formTypes } from '@constants/forms';
import { getStandardServiceHeaders } from '@helpers/fetch';
import { getGenericFormError } from '@helpers/util/form';
import { selectForm } from '@selectors/forms';
import { getServiceErrorDataValidationErrors } from '@services/index';
import { FormWithErrorSummary } from '@components/FormWithErrorSummary';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { postGroupFolder } from '@services/postGroupFolder';
import { putGroupFolder } from '@services/putGroupFolder';
import { FormErrors, FormConfig } from '@appTypes/form';

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

    const formConfig: FormConfig = selectForm(forms, formTypes.GROUP_FOLDER);
    const [errors, setErrors] = useState(formConfig?.errors);

    const { text } = folder ?? {};
    const { name } = text ?? {};
    const {secondaryHeading} = contentText ?? {};

    const handleSubmit = async (formData: FormData): Promise<FormErrors> => {

        return new Promise((resolve) => {

            const serviceToUse = router.asPath.indexOf('/update') > -1 ? putGroupFolder : postGroupFolder;
            const headers = getStandardServiceHeaders({ csrfToken, etag });

            serviceToUse({ groupId, folderId, user, headers, body: formData }).then((folderId: any) => {

                router.push(folderId ? `${routes.groupFoldersRoot}/${folderId}` : routes.groupFolder || routes.groupFoldersRoot);

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
                                    submitButton: 'Save and continue',
                                    cancelButton: 'Discard'
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
