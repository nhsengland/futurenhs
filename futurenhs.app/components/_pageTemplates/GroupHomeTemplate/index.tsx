import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { LayoutColumn } from '@components/LayoutColumn';

import { Props } from './interfaces';

export const GroupHomeTemplate: (props: Props) => JSX.Element = ({
    user,
    contentText,
    entityText,
    image,
    theme,
    actions
}) => {

    return (

        <GroupLayout 
            id="index"
            user={user}
            text={entityText}
            image={image}
            actions={actions}
            theme={theme}
            className="u-bg-theme-3">
                <LayoutColumn tablet={8} className="c-page-body">
                    
                </LayoutColumn>
        </GroupLayout>

    )

}