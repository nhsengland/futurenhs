import { formTypes } from '@constants/forms';
import { FormWithErrorSummary } from '@components/FormWithErrorSummary';
import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { RichText } from '@components/RichText';

import { Props } from './interfaces';

/**
 * Group create folder template
 */
export const GroupCreateFolderTemplate: (props: Props) => JSX.Element = ({
    csrfToken,
    forms,
    user,
    actions,
    text,
    image
}) => {

    const fields = forms?.[formTypes.CREATE_FOLDER]?.steps[0]?.fields;

    return (

        <GroupLayout 
            id="files"
            user={user}
            actions={actions}
            text={text}
            image={image} 
            className="u-bg-theme-3">            
                <LayoutColumn className="c-page-body">
                    <LayoutColumnContainer>
                        <LayoutColumn tablet={8}>
                            <FormWithErrorSummary
                                csrfToken={csrfToken}
                                fields={fields}
                                errors={{}}
                                text={{
                                    errorSummary: {
                                        body: 'There is a problem'
                                    },
                                    form: {
                                        submitButton: 'Save and continue'
                                    }
                                }} 
                                submitAction={() => {}}
                                bodyClassName="u-mb-14 u-px-14 u-pt-12 u-pb-8 u-bg-theme-1"
                                submitButtonClassName="u-float-right">
                                    <RichText wrapperElementType="p" bodyHtml="Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt" />
                            </FormWithErrorSummary>
                        </LayoutColumn>
                    </LayoutColumnContainer>
                </LayoutColumn>
        </GroupLayout>

    )

}
