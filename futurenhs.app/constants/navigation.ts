import { routes } from '@constants/routes'

export const footerNavMenuList = [
    {
        url: routes.CONTACT_US,
        text: 'Contact us',
        isActive: false,
    },
    {
        url: routes.COOKIES,
        text: 'Cookies',
        isActive: false,
    },
    {
        url: routes.PRIVACY_POLICY,
        text: 'Privacy policy',
        isActive: false,
    },
    {
        url: routes.TERMS_AND_CONDITIONS,
        text: 'Terms and conditions',
        isActive: false,
    },
    {
        url: routes.ACCESSIBILITY,
        text: 'Accessibility statement',
        isActive: false,
    },
]

export const mainNavMenuList = [
    /**
     * Temporarily removed while purpose and content is established for this site index route
     */
    // {
    //     url: routes.HOME,
    //     text: 'Home',
    //     isActive: false,
    //     isActiveRoot: false,
    //     meta: {
    //         themeId: 8,
    //         iconName: 'icon-home'
    //     }
    // },
    {
        url: routes.GROUPS,
        text: 'Groups',
        isActive: false,
        isActiveRoot: false,
        meta: {
            themeId: 14,
            iconName: 'icon-groups',
        },
    },
]
