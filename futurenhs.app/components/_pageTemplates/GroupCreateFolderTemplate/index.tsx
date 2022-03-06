import { useState } from 'react';
import { useRouter } from 'next/router';

import { routeParams } from '@constants/routes';
import { formTypes } from '@constants/forms';
import { getRouteToParam } from '@helpers/routing/getRouteToParam';
import { selectFormDefaultFields, selectFormInitialValues, selectFormErrors } from '@selectors/forms';
import { FormWithErrorSummary } from '@components/FormWithErrorSummary';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { RichText } from '@components/RichText';
import { postGroupFolder } from '@services/postGroupFolder';
import { FormErrors } from '@appTypes/form';

import { Props } from './interfaces';

/**
 * Group create folder template
 */
export const GroupCreateFolderTemplate: (props: Props) => JSX.Element = ({
    groupId,
    folderId,
    csrfToken,
    forms,
    user,
    folder,
    contentText
}) => {

    const router = useRouter();
    const [errors, setErrors] = useState(selectFormErrors(forms, formTypes.CREATE_FOLDER));

    const initialValues = selectFormInitialValues(forms, formTypes.CREATE_FOLDER);
    const fields = selectFormDefaultFields(forms, formTypes.CREATE_FOLDER);

    const { text } = folder ?? {};
    const { name } = text ?? {};

    const {secondaryHeading} = contentText ?? {};

    const groupBasePath: string = getRouteToParam({
        router: router,
        paramName: routeParams.GROUPID,
        shouldIncludeParam: true
    });

    const folderHref: string = `${groupBasePath}/folders${folderId ? '/' + folderId : ''}`;

    const handleSubmit = async (formData: FormData): Promise<FormErrors> => {

        return new Promise((resolve) => {

            postGroupFolder({ groupId, folderId, user, csrfToken, body: formData }).then(() => {

                router.push(folderHref);

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

    }

    return (

        <>
            <LayoutColumn className="c-page-body">
                <LayoutColumnContainer>
                    <LayoutColumn tablet={8}>
                        <FormWithErrorSummary
                            csrfToken={csrfToken}
                            formId={formTypes.CREATE_FOLDER}
                            fields={fields}
                            errors={errors}
                            initialValues={initialValues}
                            text={{
                                errorSummary: {
                                    body: 'There is a problem'
                                },
                                form: {
                                    submitButton: 'Save and continue',
                                    cancelButton: 'Cancel'
                                }
                            }}
                            submitAction={handleSubmit}
                            cancelHref={folderHref}
                            bodyClassName="u-mb-14 u-p-4 tablet:u-px-14 tablet:u-pt-12 u-pb-8 u-bg-theme-1"
                            submitButtonClassName="u-float-right">
                                {name &&
                                    <>
                                        <h2 className="nhsuk-heading-l o-truncated-text-lines-3">{name}</h2>
                                        <hr />
                                    </>
                                }
                                {name ? <h3 className="nhsuk-heading-m">{secondaryHeading}</h3> : <h2 className="nhsuk-heading-l">{secondaryHeading}</h2>}
                                <RichText wrapperElementType="p" bodyHtml="Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt" />
                        </FormWithErrorSummary>
                    </LayoutColumn>
                </LayoutColumnContainer>
            </LayoutColumn>
        </>

    )

}
