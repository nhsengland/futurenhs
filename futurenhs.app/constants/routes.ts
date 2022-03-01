export const enum routes {
    HOME = '/',
    ADMIN = '/admin',
    ADMIN_USERS = '/admin/users',
    ADMIN_GROUPS = '/admin/groups',
    USERS = '/users',
    LOG_OUT = '/log-out',
    GROUPS = '/groups',
    ACCESSIBILITY = '/accessibility-statement',
    CONTACT_US = '/contact-us',
    COOKIES = '/cookies',
    PRIVACY_POLICY = '/privacy-policy',
    TERMS_AND_CONDITIONS = '/terms-and-conditions',
    API_LOGIN = '/api/log-in'
}

export const enum routeParams {
    GROUPID = 'groupId',
    FOLDERID = 'folderId',
    FILEID = 'fileId',
    MEMBERID = 'memberId',
    DISCUSSIONID = 'discussionId',
}

export const enum layoutIds {
    BASE = 'base',
    GROUP = 'group',
    ADMIN = 'admin'
}