import React from 'react'
import { GroupPageHeader } from './index'
import { actions as actionsConstants } from '@constants/actions'
import { useAssetPath } from '@hooks/useAssetPath'
import { routes } from '@jestMocks/generic-props'

const exampleImageSrc: string = useAssetPath('/images/example-group-image.svg')

export default {
    title: 'GroupPageHeader',
    component: GroupPageHeader,
    argTypes: {
        themeId: {
            options: [
                '36d49305-eca8-4176-bfea-d25af21469b9',
                '9a3c911b-c3d3-4f58-a32a-d541e0f5bf56',
                '5053a8c6-ea4d-4125-9dc3-475e3e931fee',
            ],
            control: { type: 'radio' },
        },
        id: {
            control: { type: '' },
        },
        actions: {
            control: { type: '' },
        },
        routes: {
            control: { type: '' },
        },
        image: {
            control: { type: '' },
        },
    },
}

const Template = (args) => (
    <div className="l-col-container u-flex-row u-justify-start">
        <GroupPageHeader {...args} />
    </div>
)

export const Basic = Template.bind({})
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
            url: '/',
        },
        {
            isActive: false,
            text: 'Discover new groups',
            url: '/',
        },
    ],
    routes: routes,
}

export const GroupAdmin = Template.bind({})
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
            url: '/',
        },
        {
            isActive: false,
            text: 'Forum',
            url: '/forum',
        },
        {
            isActive: false,
            text: 'Files',
            url: '/files',
        },
        {
            isActive: false,
            text: 'Members',
            url: '/',
        },
    ],
    actions: [actionsConstants.GROUPS_EDIT],
    routes: routes,
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
            url: '/',
        },
        {
            isActive: false,
            text: 'Forum',
            url: '/forum',
        },
        {
            isActive: false,
            text: 'Files',
            url: '/files',
        },
        {
            isActive: false,
            text: 'Members',
            url: '/',
        },
    ],
    actions: [actionsConstants.GROUPS_LEAVE],
    routes: routes,
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
            url: '/',
        },
        {
            isActive: false,
            text: 'Forum',
            url: '/forum',
        },
        {
            isActive: false,
            text: 'Files',
            url: '/files',
        },
        {
            isActive: false,
            text: 'Members',
            url: '/',
        },
    ],
    actions: [actionsConstants.GROUPS_JOIN],
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
            url: '/',
        },
        {
            isActive: false,
            text: 'Forum',
            url: '/forum',
        },
        {
            isActive: false,
            text: 'Files',
            url: '/files',
        },
        {
            isActive: false,
            text: 'Members',
            url: '/',
        },
    ],
    actions: [actionsConstants.GROUPS_LEAVE],
    routes: routes,
    image: {
        src: exampleImageSrc,
        altText: 'Group logo',
        height: '180px',
        width: '180px',
    },
}

export const Theme2 = Template.bind({})
Theme2.args = {
    text: {
        mainHeading: 'Name of group',
        description: 'Group description. Example with image.',
        navMenuTitle: 'Nav menu aria label',
    },
    navMenuList: [
        {
            isActive: true,
            text: 'Home',
            url: '/',
        },
        {
            isActive: false,
            text: 'Forum',
            url: '/forum',
        },
        {
            isActive: false,
            text: 'Files',
            url: '/files',
        },
        {
            isActive: false,
            text: 'Members',
            url: '/',
        },
    ],
    actions: [actionsConstants.GROUPS_LEAVE],
    routes: routes,
    themeId: '9a3c911b-c3d3-4f58-a32a-d541e0f5bf56',
}

export const Theme3 = Template.bind({})
Theme3.args = {
    text: {
        mainHeading: 'Name of group',
        description: 'Group description. Example with image.',
        navMenuTitle: 'Nav menu aria label',
    },
    navMenuList: [
        {
            isActive: true,
            text: 'Home',
            url: '/',
        },
        {
            isActive: false,
            text: 'Forum',
            url: '/forum',
        },
        {
            isActive: false,
            text: 'Files',
            url: '/files',
        },
        {
            isActive: false,
            text: 'Members',
            url: '/',
        },
    ],
    actions: [actionsConstants.GROUPS_LEAVE],
    routes: routes,
    themeId: '5053a8c6-ea4d-4125-9dc3-475e3e931fee',
}
