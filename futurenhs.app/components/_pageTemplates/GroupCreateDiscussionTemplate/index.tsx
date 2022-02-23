import { useState } from 'react';
import { useRouter } from 'next/router';

import { routeParams } from '@constants/routes';
import { selectFormDefaultFields, selectFormErrors, selectFormInitialValues } from '@selectors/forms';
import { getRouteToParam } from '@helpers/routing/getRouteToParam';
import { formTypes } from '@constants/forms';
import { FormWithErrorSummary } from '@components/FormWithErrorSummary';
import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { postGroupDiscussion } from '@services/postGroupDiscussion';

import { Props } from './interfaces';

/**
 * Group create discussion template
 */
export const GroupCreateDiscussionTemplate: (props: Props) => JSX.Element = ({
    groupId,
    csrfToken,
    themeId,
    forms,
    user,
    actions,
    contentText,
    entityText,
    image,
    services = {
        postGroupDiscussion: postGroupDiscussion
    }
}) => {

    const router = useRouter();
    const [errors, setErrors] = useState(selectFormErrors(forms, formTypes.CREATE_DISCUSSION));

    const fields = selectFormDefaultFields(forms, formTypes.CREATE_DISCUSSION);
    const initialValues = selectFormInitialValues(forms, formTypes.CREATE_DISCUSSION);

    const groupBasePath: string = getRouteToParam({
        router: router,
        paramName: routeParams.GROUPID,
        shouldIncludeParam: true
    });

    const forumHref: string = `${groupBasePath}/forum`;

    /**
     * Client-side submission handler
     */
    const handleSubmit = async (submission) => {

        try {

            const response = await services.postGroupDiscussion({
                groupId: groupId,
                user: user,
                csrfToken: csrfToken,
                body: {
                    formId: formTypes.CREATE_DISCUSSION,
                    ...submission
                }
            });

            router.push(forumHref);

        } catch(error){

            setErrors({
                [error.data.status]: error.data.statusText
            });

        }

    };

    /**
     * Render
     */
    return (

        <GroupLayout 
            id="forum"
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
                                formId={formTypes.CREATE_DISCUSSION}
                                fields={fields}
                                initialValues={initialValues}
                                errors={errors}
                                text={{
                                    errorSummary: {
                                        body: 'There is a problem'
                                    },
                                    form: {
                                        submitButton: 'Create discussion',
                                        cancelButton: 'Cancel'
                                    }
                                }} 
                                submitAction={handleSubmit}
                                cancelHref={forumHref}
                                bodyClassName="u-mb-14 u-p-4 tablet:u-px-14 tablet:u-pt-12 u-pb-8 u-bg-theme-1">
                                    <h2>Create discussion</h2>
                            </FormWithErrorSummary>
                        </LayoutColumn>
                    </LayoutColumnContainer>
                </LayoutColumn>
        </GroupLayout>

    )

}
