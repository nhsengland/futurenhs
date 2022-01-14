import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { LayoutColumn } from '@components/LayoutColumn';

import { Props } from './interfaces';  

/**
 * Group member template
 */
export const GroupMemberTemplate: (props: Props) => JSX.Element = ({
    user,
    content,
    image
}) => {

    return (

        <GroupLayout 
            id="members"
            user={user}
            content={content}
            image={image} 
            className="u-bg-theme-3">
                <LayoutColumn className="u-px-4 u-py-10">
                    USER
                </LayoutColumn>
        </GroupLayout>

    )

}