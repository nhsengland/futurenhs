export interface Routes {
    siteRoot: string
    usersRoot: string
    adminRoot: string
    adminUsersRoot: string
    adminUsersInvite: string
    adminGroupsRoot: string
    adminGroupsCreate: string
    groupsRoot: string
    groupsDiscover: string
    groupRoot: string
    groupJoin: string
    groupLeave: string
    groupUpdate: string
    groupInvite: string
    groupForumRoot: string
    groupFoldersRoot: string
    groupFolder: string
    groupFilesRoot: string
    groupMembersRoot: string
    groupAboutRoot: string
}

export interface BreadCrumbElement {
    element: string
    text: string
}

export type BreadCrumbList = Array<BreadCrumbElement>
