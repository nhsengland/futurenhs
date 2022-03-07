import { actions as userActions } from '@constants/actions';

declare interface Config {
    groupRoute: string;
    actions: Array<userActions>;
}

export const getActionNavMenuList = ({
    groupRoute,
    actions
}: Config): Array<{
    id: userActions;
    url: string;
    text: string;
}> => {

    const actionsMenuList: Array<{
        id: userActions;
        url: string;
        text: string;
    }> = [];

    if(actions?.includes(userActions.GROUPS_EDIT)){

        actionsMenuList.push({
            id: userActions.GROUPS_EDIT,
            url: `${groupRoute}/update`,
            text: 'Edit group information'
        });

    }

    if(actions?.includes(userActions.GROUPS_LEAVE)){

        actionsMenuList.push({
            id: userActions.GROUPS_LEAVE,
            url: '/',
            text: 'Leave group'
        });

    }

    if(actions?.includes(userActions.GROUPS_MEMBERS_ADD)){

        actionsMenuList.push({
            id: userActions.GROUPS_MEMBERS_ADD,
            url: '/',
            text: 'Add new member'
        });

    }

    if(actions?.includes(userActions.SITE_MEMBERS_ADD)){

        actionsMenuList.push({
            id: userActions.SITE_MEMBERS_ADD,
            url: '/',
            text: 'Invite new user'
        });

    }

    return actionsMenuList;

};
