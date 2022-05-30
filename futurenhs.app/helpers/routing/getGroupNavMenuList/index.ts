declare interface Config {
    groupRoute: string
    activeId?: 'index' | 'forum' | 'files' | 'members' | 'about'
    isRestricted?: boolean
}

export const getGroupNavMenuList = ({
    groupRoute,
    activeId,
    isRestricted,
}: Config): Array<{
    url: string
    text: string
    isActive: boolean
}> => {
    const navConfig: Array<{
        id: Config['activeId']
        url: string
        text: string
    }> = isRestricted
        ? [
              {
                  id: 'about',
                  url: `${groupRoute}/about`,
                  text: 'About',
              },
          ]
        : [
              {
                  id: 'index',
                  url: groupRoute,
                  text: 'Home',
              },
              {
                  id: 'forum',
                  url: `${groupRoute}/forum`,
                  text: 'Forum',
              },
              {
                  id: 'files',
                  url: `${groupRoute}/folders`,
                  text: 'Files',
              },
              {
                  id: 'members',
                  url: `${groupRoute}/members`,
                  text: 'Members',
              },
          ]

    return navConfig.map(({ id, url, text }) => ({
        url: url,
        text: text,
        isActive: id === activeId,
    }))
}
