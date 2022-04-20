import React from 'react';

import { BreadCrumb } from './index';

export default {
    title: 'Breadcrumb',
    component: BreadCrumb,
};

const Template = (args) => <BreadCrumb {...args} />

export const Basic = Template.bind({});
Basic.args = {
    breadCrumbList: [
        {
            element: '/',
            text: 'Home'
        },
        {
            element: '/child-1',
            text: 'Child 1'
        },
        {
            element: '/child-2',
            text: 'Child 2'
        }
    ],
    text: {
        ariaLabel: 'Site breadcrumb'
    },
    className: 'u-fill-theme-5'
}
