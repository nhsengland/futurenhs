import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { LayoutColumn } from '@components/LayoutColumn';

import { Props } from './interfaces';

export const GroupHomeTemplate: (props: Props) => JSX.Element = ({
    user,
    content,
    image
}) => {

    return (

        <GroupLayout 
            id="index"
            user={user}
            content={content}
            image={image} 
            className="u-bg-theme-3">
                <LayoutColumn tablet={8} className="u-px-4 u-py-10">
                    
                </LayoutColumn>
        </GroupLayout>

    )

}