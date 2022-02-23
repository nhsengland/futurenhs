import { useState } from 'react';
import { useRouter } from 'next/router';

import { routeParams } from '@constants/routes';
import { formTypes } from '@constants/forms';
import { getRouteToParam } from '@helpers/routing/getRouteToParam';
import { selectFormDefaultFields, selectFormInitialValues, selectFormErrors } from '@selectors/forms';
import { FormWithErrorSummary } from '@components/FormWithErrorSummary';
import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { RichText } from '@components/RichText';
import { postGroupFolder } from '@services/postGroupFolder';

import { Props } from './interfaces';

/**
 * Group create folder template
 */
export const GroupCreateFolderTemplate: (props: Props) => JSX.Element = ({
    groupId,
    themeId,
    csrfToken,
    forms,
    user,
    actions,
    folder,
    contentText,
    entityText,
    image
}) => {

    const router = useRouter();
    const [errors, setErrors] = useState(selectFormErrors(forms, formTypes.CREATE_FOLDER));

    const initialValues = selectFormInitialValues(forms, formTypes.CREATE_FOLDER);
    const fields = selectFormDefaultFields(forms, formTypes.CREATE_FOLDER);

    const { text } = folder ?? {};
    const { name } = text ?? {};

    const groupBasePath: string = getRouteToParam({
        router: router,
        paramName: routeParams.GROUPID,
        shouldIncludeParam: true
    });

    const folderHref: string = `${groupBasePath}/folders`;

    const handleSubmit = async (submission) => {

        try {

            const response = await postGroupFolder({
                groupId: groupId,
                user: user,
                csrfToken: csrfToken,
                body: submission
            });

            router.push(folderHref);

        } catch(error){

            setErrors({
                [error.data.status]: error.data.statusText
            });

        }

    }

    return (

        <GroupLayout 
            id="files"
            user={user}
            actions={actions}
            text={entityText}
            image={image} 
            themeId={themeId}
            className="u-bg-theme-3">            
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
                                bodyClassName="u-mb-14 u-px-14 u-pt-12 u-pb-8 u-bg-theme-1"
                                submitButtonClassName="u-float-right">
                                    {name &&
                                        <>
                                            <h2 className="o-truncated-text-lines-3">{name}</h2>
                                            <hr />
                                        </>
                                    }
                                    {name ? <h3>Add Folder</h3> : <h2>Add Folder</h2>}
                                    <RichText wrapperElementType="p" bodyHtml="Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt" />
                            </FormWithErrorSummary>
                        </LayoutColumn>
                    </LayoutColumnContainer>
                </LayoutColumn>
        </GroupLayout>

    )

}
