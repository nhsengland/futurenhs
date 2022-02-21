import { AdminLayout } from '@components/_pageLayouts/AdminLayout';

import { Props } from './interfaces';

/**
 * Admin groups dashboard template
 */
export const AdminGroupsTemplate: (props: Props) => JSX.Element = ({
    contentText,
    user,
    actions
}) => {

    const generatedClasses = {

    };

    return (

        <AdminLayout
            user={user}
            actions={actions}
            contentText={contentText}
            className="u-bg-theme-3">
                <h2>Groups</h2>
        </AdminLayout>

    )

}
