
import { actions as actionConstants } from '@constants/actions';
import { routes } from '@constants/routes';
import { Link } from '@components/Link';
import { AdminLayout } from '@components/_pageLayouts/AdminLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { Card } from '@components/Card';

import { Props } from './interfaces';

/**
 * Admin home dashboard template
 */
export const AdminHomeTemplate: (props: Props) => JSX.Element = ({
    contentText,
    user,
    actions
}) => {

    const generatedClasses = {

    };

    const shouldRenderUsersLink: boolean = actions.includes(actionConstants.SITE_ADMIN_MEMBERS_ADD) || actions.includes(actionConstants.SITE_ADMIN_MEMBERS_EDIT) || actions.includes(actionConstants.SITE_ADMIN_MEMBERS_DELETE);
    const shouldRenderGroupsLink: boolean = actions.includes(actionConstants.SITE_ADMIN_GROUPS_ADD) || actions.includes(actionConstants.SITE_ADMIN_GROUPS_EDIT) || actions.includes(actionConstants.SITE_ADMIN_GROUPS_DELETE);

    return (

        <AdminLayout
            user={user}
            actions={actions}
            contentText={contentText}
            className="u-bg-theme-3">
                <LayoutColumnContainer>
                    {shouldRenderUsersLink &&
                        <LayoutColumn tablet={4}>
                            <Card clickableHref={routes.ADMIN_USERS}>
                                <h2 className="nhsuk-card__heading nhsuk-heading-m"><Link href={routes.ADMIN_USERS}>Manage users</Link></h2>
                            </Card>
                        </LayoutColumn>
                    }
                    {shouldRenderGroupsLink &&
                        <LayoutColumn tablet={4}>
                            <Card clickableHref={routes.ADMIN_GROUPS}>
                                <h2 className="nhsuk-card__heading nhsuk-heading-m"><Link href={routes.ADMIN_GROUPS}>Manage groups</Link></h2>
                            </Card>
                        </LayoutColumn>
                    }
                </LayoutColumnContainer>
        </AdminLayout>

    )

}
