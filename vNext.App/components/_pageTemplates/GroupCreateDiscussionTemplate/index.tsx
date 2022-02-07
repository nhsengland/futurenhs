import { useRouter } from 'next/router';

import { routeParams } from '@constants/routes';
import { getRouteToParam } from '@helpers/routing/getRouteToParam';
import { formTypes } from '@constants/forms';
import { FormWithErrorSummary } from '@components/FormWithErrorSummary';
import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';

import { Props } from './interfaces';

/**
 * Group create discussion template
 */
export const GroupCreateDiscussionTemplate: (props: Props) => JSX.Element = ({
    csrfToken,
    forms,
    user,
    actions,
    contentText,
    entityText,
    image,
    errors
}) => {

    const router = useRouter();

    const fields = forms?.[formTypes.CREATE_DISCUSSION]?.steps[0].fields ?? [];

    const groupBasePath: string = getRouteToParam({
        router: router,
        paramName: routeParams.GROUPID,
        shouldIncludeParam: true
    });

    const cancelHref: string = `${groupBasePath}/forum`;

    return (

        <GroupLayout 
            id="forum"
            user={user}
            actions={actions}
            text={entityText}
            image={image} 
            className="u-bg-theme-3">            
                <LayoutColumn className="c-page-body">
                    <LayoutColumnContainer>
                        <LayoutColumn tablet={8}>
                            <FormWithErrorSummary
                                csrfToken={csrfToken}
                                fields={fields}
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
                                submitAction={console.log}
                                cancelHref={cancelHref}
                                bodyClassName="u-mb-14 u-px-14 u-pt-12 u-pb-8 u-bg-theme-1">
                                    <h2>Create discussion</h2>
                            </FormWithErrorSummary>
                        </LayoutColumn>
                    </LayoutColumnContainer>
                </LayoutColumn>
        </GroupLayout>

    )

}
