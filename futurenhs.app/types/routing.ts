export interface Routes {
    siteRoot: string;
    adminRoot: string;
    adminUsersRoot: string;
    adminUsersInvite: string;
    adminGroupsRoot: string;
    adminGroupsCreate: string;
    groupsRoot: string;
    groupsDiscover: string;
    groupRoot: string;
    groupJoin: string;
    groupLeave: string;
    groupUpdate: string;
    groupForumRoot: string;
    groupFoldersRoot: string;
    groupFolder: string;
    groupFilesRoot: string;
    groupMembersRoot: string;
}

export interface BreadCrumbElement {
    element: string;
    text: string;
}

export type BreadCrumbList = Array<BreadCrumbElement>;
 