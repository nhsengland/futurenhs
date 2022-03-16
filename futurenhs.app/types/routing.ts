export interface Routes {
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
 