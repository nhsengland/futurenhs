import { actions as userActions } from '@constants/actions';

declare interface Config {
    actions: Array<userActions>;
}

export const getActionNavMenuList = ({
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
            url: '/',
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
