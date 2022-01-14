import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { Avatar } from '@components/Avatar';

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
                <div className="u-px-4 u-py-10 u-w-full">
                    <LayoutColumnContainer justify="centre">
                        <LayoutColumn tablet={3} desktop={2}>
                            <Avatar initials="ri" />
                        </LayoutColumn>
                        <LayoutColumn tablet={7} desktop={8}>
                            <h2>My profile</h2>
                            <dl>
                                <dt className="u-text-bold">First name</dt>
                                <dd>Richard</dd>
                                <dt className="u-text-bold">Last name</dt>
                                <dd>Iles</dd>
                                <dt className="u-text-bold">Preferred pronouns</dt>
                                <dd>he/ him</dd>
                                <dt className="u-text-bold">Email address</dt>
                                <dd>richard.iles@cds.co.uk</dd>
                            </dl>
                        </LayoutColumn>
                    </LayoutColumnContainer>
                </div>
        </GroupLayout>

    )

}