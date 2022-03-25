import { Routes } from "@appTypes/routing";

export const routes: Routes = {
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