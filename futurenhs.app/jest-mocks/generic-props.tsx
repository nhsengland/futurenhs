import { Routes } from "@appTypes/routing";

export const routes: Routes = {
    siteRoot: '/',
    groupsRoot: '/groups',
    groupsDiscover: '/groups/discover',
    groupRoot: '/:groupId',
    groupUpdate: '/:groupId/update',
    groupJoin: '/:groupId/join',
    groupLeave: '/:groupId/leave',
    groupForumRoot: '/:groupId/forum',
    groupFoldersRoot: '/:groupId/folders',
    groupFolder: '/:groupId/folders/:folderId',
    groupFilesRoot: '/:groupId/files',
    groupMembersRoot: '/:groupId/members',
};