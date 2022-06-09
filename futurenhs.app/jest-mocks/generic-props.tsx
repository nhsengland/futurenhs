import { Routes } from '@appTypes/routing'

export const routes: Routes = {
    siteRoot: '/',
    usersRoot: '/users',
    adminRoot: '/admin',
    adminUsersRoot: '/admin/users',
    adminUsersInvite: '/admin/users/invite',
    adminGroupsRoot: '/admin/groups',
    adminGroupsCreate: '/admin/users/create',
    authApiSignInAzureB2C: '/api/auth/signin/azure-ad-b2c',
    authApiSignOut: '/api/auth/signout',
    authSignIn: '/auth/signin',
    authSignOut: '/auth/signout',
    groupsRoot: '/groups',
    groupsDiscover: '/groups/discover',
    groupRoot: '/:groupId',
    groupUpdate: '/:groupId/update',
    groupInvite: '/:groupId/invite',
    groupJoin: '/:groupId/join',
    groupLeave: '/:groupId/leave',
    groupForumRoot: '/:groupId/forum',
    groupFoldersRoot: '/:groupId/folders',
    groupFolder: '/:groupId/folders/:folderId',
    groupFilesRoot: '/:groupId/files',
    groupMembersRoot: '/:groupId/members',
    groupAboutRoot: '/:groupId/about'
};
