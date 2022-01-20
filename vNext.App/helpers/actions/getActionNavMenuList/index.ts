import { actions as userActions } from '@constants/actions';

declare interface Config {
    groupRoute: string;
    actions: Array<userActions>;
}

export const getActionNavMenuList = ({
    groupRoute,
    actions
}: Config): Array<{
    url: string;
    text: string;
}> => {

    const actionsMenuList: Array<{
        url: string;
        text: string;
    }> = [];

    if(actions?.includes(userActions.GROUPS_EDIT)){

        actionsMenuList.push({
            url: `${groupRoute}/update`,
            text: 'Edit group information'
        });

    }

    actionsMenuList.push({
        url: '/',
        text: 'Leave group'
    });

    if(actions?.includes(userActions.GROUPS_MEMBERS_ADD)){

        actionsMenuList.push({
            url: '/',
            text: 'Invite new member'
        });

    }

    if(actions?.includes(userActions.SITE_MEMBERS_ADD)){

        actionsMenuList.push({
            url: '/',
            text: 'Invite new user'
        });

    }

    return actionsMenuList;

};
