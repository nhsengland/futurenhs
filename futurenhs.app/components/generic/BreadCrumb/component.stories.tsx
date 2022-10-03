import React from 'react'

import { BreadCrumb } from './index'

export default {
    title: 'Breadcrumb',
    component: BreadCrumb,
    argTypes: {
        breadCrumbList: {
            control: { type: '' },
        },
        className: {
            control: { type: '' },
        },
        seperatorIconName: {
            control: { type: 'select' },
            options: (window.parent as any).sbIconList,
        },
    },
}

const Template = (args) => <BreadCrumb {...args} />

export const Basic = Template.bind({})
Basic.args = {
    breadCrumbList: [
        {
            element: '/',
            text: 'Home',
        },
        {
            element: '/child-1',
            text: 'Child 1',
        },
        {
            element: '/child-2',
            text: 'Child 2',
        },
    ],
    text: {
        ariaLabel: 'Site breadcrumb',
    },
    className: 'u-fill-theme-5',
}

export const Truncated = Template.bind({})
Truncated.args = {
    truncationMinPathLength: 3,
    truncationStartIndex: 0,
    truncationEndIndex: 1,
    breadCrumbList: [
        {
            element: '/',
            text: 'Home',
        },
        {
            element: '/child-1',
            text: 'Child 1',
        },
        {
            element: '/child-2',
            text: 'Child 2',
        },
        {
            element: '/child-3',
            text: 'Child 3',
        },
    ],
    text: {
        ariaLabel: 'Site breadcrumb',
    },
    className: 'u-fill-theme-5',
}
