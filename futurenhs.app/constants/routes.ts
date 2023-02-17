export const enum routes {
    HOME = '/',
    ADMIN = '/admin',
    ADMIN_USERS = '/admin/users',
    ADMIN_GROUPS = '/admin/groups',
    ADMIN_DOMAINS = '/admin/domains',
    SITE_FEATURES = '/admin/features',
    ADMIN_ANALYTICS = '/admin/analytics',
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
    GROUP_INVITES = '/v1/groups/invites',
    GROUP_INVITE_ID = '/v1/groups/invite/%INVITE_ID%',
    POST_GROUP_INVITE = '/v1/groups/%GROUP_ID%/registration/invite',

    SITE_FEATURE_FLAGS = '/v1/features',

    FEATURE_FLAG_SLUG = '/v1/features/%FLAG%',
    SITE_INVITE = '/v1/registration/invite',
    SITE_INVITE_ID = '/v1/registration/invite/%INVITE_ID%',
    SITE_USER_REGISTER = '/v1/registration/register',
    SITE_IDENTITY = '/v1/registration/identity',
    SITE_DOMAINS = '/v1/registration/domains',
    SITE_DOMAIN_ID = '/v1/registration/domains/%DOMAIN_ID%',
    SITE_PUBLIC_REGISTRATION_EXISTS = '/v1/registration/public/exists',

    // ADMIN_FEATURE_FLAGS = '/v1/admin/features',
    ADMIN_ANALYTICS = '/v1/admin/analytics',
}

export const enum features {
    SELF_REGISTER = 'SelfRegistration',
    GROUP_INVITE = 'GroupInvite',
    FILE_EDIT = 'FileServer-AllowFileEdit',
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
