export interface Routes {
    groupRoot: string;
    groupForumRoot: string;
    groupFoldersRoot: string;
    groupFilesRoot: string;
    groupMembersRoot: string;
}

export interface BreadCrumbElement {
    element: string;
    text: string;
}

export type BreadCrumbList = Array<BreadCrumbElement>;
 