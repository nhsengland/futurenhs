import { FormWithErrorSummary } from '@components/FormWithErrorSummary';
import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { RichText } from '@components/RichText';
import { createFolderForm } from '@formConfigs/create-folder';

import { Props } from './interfaces';

/**
 * Group create folder template
 */
export const GroupUpdateTemplate: (props: Props) => JSX.Element = ({
    csrfToken,
    user,
    actions,
    contentText,
    entityText,
    image
}) => {

    return (

        <GroupLayout 
            id="index"
            user={user}
            actions={actions}
            text={entityText}
            image={image} 
            className="u-bg-theme-3">            
                <LayoutColumn className="c-page-body">
                    <LayoutColumnContainer>
                        <LayoutColumn tablet={8}>
                            TODO
                        </LayoutColumn>
                    </LayoutColumnContainer>
                </LayoutColumn>
        </GroupLayout>

    )

}
