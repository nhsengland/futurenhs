import { routes } from '@constants/routes';

export const footerNavMenuList = [
    {
        url: routes.ACCESSIBILITY,
        text: 'Accessibility Statement',
        isActive: false
    },
    {
        url: routes.CONTACT_US,
        text: 'Contact us',
        isActive: false
    },
    {
        url: routes.COOKIES,
        text: 'Cookies',
        isActive: false
    },
    {
        url: routes.PRIVACY_POLICY,
        text: 'Privacy policy',
        isActive: false
    },
    {
        url: routes.TERMS_AND_CONDITIONS,
        text: 'Terms and conditions',
        isActive: false
    }
];

export const mainNavMenuList = [
    {
        url: routes.HOME,
        text: 'Home',
        isActive: false,
        meta: {
            themeId: 8,
            iconName: 'icon-home'
        }
    },
    {
        url: routes.GROUPS,
        text: 'Groups',
        isActive: false,
        meta: {
            themeId: 11,
            iconName: 'icon-group'
        }
    }
];
