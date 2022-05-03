import React from 'react'
import { GroupPageHeader } from './index'
import { actions as actionsConstants } from '@constants/actions'
import { routes } from '@jestMocks/generic-props';
import { defaultGroupLogos } from '@constants/icons';

export default {
    title: 'GroupPageHeader',
    component: GroupPageHeader,
    argTypes: {
        themeId: {
            options: ['36d49305-eca8-4176-bfea-d25af21469b9', '9a3c911b-c3d3-4f58-a32a-d541e0f5bf56', '5053a8c6-ea4d-4125-9dc3-475e3e931fee'],
            control: { type: 'radio' },
        },
        id: {
            control: { type: '' }
        },
        actions: {
            control: { type: '' }
        },
        routes: {
            control: { type: '' }
        },
        image: {
            control: { type: '' }
        }
    },

};

const Template = (args) => <div className='l-col-container u-flex-row u-justify-start'><GroupPageHeader {...args} /></div>

export const Basic = Template.bind({});
Basic.args = {
    text: {
        mainHeading: 'Discover new Groups',
        description: 'Search for new groups to join',
        navMenuTitle: 'Nav menu aria label',
    },
    navMenuList: [
        {
            isActive: true,
            text: 'My groups',
            url: '/'
        },
        {
            isActive: false,
            text: 'Discover new groups',
            url: '/'
        }
    ],
}

export const GroupAdmin = Template.bind({});
GroupAdmin.args = {
    text: {
        mainHeading: 'Name of group',
        description: 'Group description. Admin view, select actions.',
        navMenuTitle: 'Nav menu aria label',
    },
    navMenuList: [
        {
            isActive: true,
            text: 'Home',
            url: '/'
        },
        {
            isActive: false,
            text: 'Forum',
            url: '/forum'
        },
        {
            isActive: false,
            text: 'Files',
            url: '/files'
        },
        {
            isActive: false,
            text: 'Members',
            url: '/'
        },
    ],
    actions: [
        actionsConstants.GROUPS_EDIT
    ],
    routes: routes,
    themeId: '9a3c911b-c3d3-4f58-a32a-d541e0f5bf56'
}

export const GroupMember = Template.bind({})
GroupMember.args = {
    text: {
        mainHeading: 'Name of group',
        description: 'Group description. Member view, select actions.',
        navMenuTitle: 'Nav menu aria label',
    },
    navMenuList: [
        {
            isActive: true,
            text: 'Home',
            url: '/'
        },
        {
            isActive: false,
            text: 'Forum',
            url: '/forum'
        },
        {
            isActive: false,
            text: 'Files',
            url: '/files'
        },
        {
            isActive: false,
            text: 'Members',
            url: '/'
        },
    ],
    actions: [
        actionsConstants.GROUPS_LEAVE
    ],
    routes: routes,
    themeId: '5053a8c6-ea4d-4125-9dc3-475e3e931fee'
}

export const JoinGroup = Template.bind({})
JoinGroup.args = {
    text: {
        mainHeading: 'Name of group',
        description: 'Group description. Non member view.',
        navMenuTitle: 'Nav menu aria label',
    },
    navMenuList: [
        {
            isActive: true,
            text: 'Home',
            url: '/'
        },
        {
            isActive: false,
            text: 'Forum',
            url: '/forum'
        },
        {
            isActive: false,
            text: 'Files',
            url: '/files'
        },
        {
            isActive: false,
            text: 'Members',
            url: '/'
        },
    ],
    actions: [
        actionsConstants.GROUPS_JOIN
    ],
    routes: routes,
}

export const Image = Template.bind({})
Image.args = {
    text: {
        mainHeading: 'Name of group',
        description: 'Group description. Example with image.',
        navMenuTitle: 'Nav menu aria label',
    },
    navMenuList: [
        {
            isActive: true,
            text: 'Home',
            url: '/'
        },
        {
            isActive: false,
            text: 'Forum',
            url: '/forum'
        },
        {
            isActive: false,
            text: 'Files',
            url: '/files'
        },
        {
            isActive: false,
            text: 'Members',
            url: '/'
        },
    ],
    actions: [
        actionsConstants.GROUPS_LEAVE
    ],
    routes: routes,
    image: defaultGroupLogos.large
}

export const Truncate = Template.bind({});
Truncate.args = {
    text: {
        mainHeading: 'Long group description',
        description: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas convallis mi eget sollicitudin eleifend. Maecenas elementum orci id lectus lobortis auctor. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas.',
        navMenuTitle: 'Nav menu aria label',
    },
    navMenuList: [
        {
            isActive: true,
            text: 'Home',
            url: '/'
        },
        {
            isActive: false,
            text: 'Forum',
            url: '/forum'
        },
        {
            isActive: false,
            text: 'Files',
            url: '/files'
        },
        {
            isActive: false,
            text: 'Members',
            url: '/'
        },
    ],
    actions: [
        actionsConstants.GROUPS_JOIN
    ],
    routes: routes,
}

