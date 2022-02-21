import { AdminLayout } from '@components/_pageLayouts/AdminLayout';

import { Props } from './interfaces';

/**
 * Admin users dashboard template
 */
export const AdminUsersTemplate: (props: Props) => JSX.Element = ({
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
                <h2>Users</h2>
        </AdminLayout>

    )

}