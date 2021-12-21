import { getRouteToParam } from '@helpers/routing/getRouteToParam';

declare interface Config {
    router: any;
    activeId?: 'index' | 'forum' | 'files' | 'members';
}

export const getGroupNavMenuList = ({
    router,
    activeId
}: Config): Array<{
    url: string;
    text: string;
    isActive: boolean;
}> => {

    const baseGroupRoute: string = getRouteToParam({ 
        router, 
        paramName: 'group' 
    });

    const navConfig: Array<{
        id: Config["activeId"],
        url: string;
        text: string;
    }> = [
        {
            id: 'index',
            url: baseGroupRoute,
            text: 'Home'
        },
        {
            id: 'forum',
            url: `${baseGroupRoute}/forum`,
            text: 'Forum'
        },
        {
            id: 'files',
            url: `${baseGroupRoute}/folders`,
            text: 'Files'
        },
        {
            id: 'members',
            url: `${baseGroupRoute}/members`,
            text: 'Members'
        }
    ]

    return navConfig.map(({ id, url, text }) => ({
        url: url,
        text: text,
        isActive: id === activeId
    }));

};
