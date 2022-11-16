export const enum routes {
    HOME = '/',
    ADMIN = '/admin',
    ADMIN_USERS = '/admin/users',
    ADMIN_GROUPS = '/admin/groups',
    ADMIN_DOMAINS = '/admin/domains',
    ADMIN_FEATURES = '/admin/features',
    USERS = '/users',
    SIGN_IN = '/auth/signin',
    SIGN_OUT = '/auth/signout',
    GROUPS = '/groups',
    ACCESSIBILITY = '/accessibility-statement',
    CONTACT_US = '/contact-us',
    COOKIES = '/cookies',
    PRIVACY_POLICY = '/privacy-policy',
    TERMS_AND_CONDITIONS = '/terms-and-conditions',
}

export const enum api {
    USER_GROUP_INVITES = '/v1/groups/invites',
    USER_GROUP_INVITE = '/v1/groups/invite/%GROUP_INVITE_ID%',

    GROUP_INVITE = '/v1/groups/%GROUP_ID%/registration/invite',

    SITE_INVITE = '/v1/registration/invite',
    SITE_INVITE_DETAILS = '/v1/registration/invite/%SITE_INVITE_ID%',
    SITE_USER_REGISTER = '/v1/registration/register',
    SITE_IDENTITY = '/v1/registration/identity',
    SITE_DOMAINS = '/v1/registration/domains',
    SITE_DOMAIN = '/v1/registration/domains/%DOMAIN%',
}

export const enum queryParams {
    RETURNURL = 'returnUrl',
    EDIT = 'edit',
}

export const enum routeParams {
    GROUPID = 'groupId',
    FOLDERID = 'folderId',
    FILEID = 'fileId',
    MEMBERID = 'memberId',
    DISCUSSIONID = 'discussionId',
    USERID = 'userId',
}

export const enum layoutIds {
    BASE = 'base',
    GROUP = 'group',
    ADMIN = 'admin',
}

export const enum groupTabIds {
    INDEX = 'index',
    FORUM = 'forum',
    FILES = 'files',
    WHITEBOARD = 'whiteboard',
    MEMBERS = 'members',
    ABOUT = 'about',
}
