import React from "react"

import { Footer } from "./index"

export default {
    title: 'Footer',
    component: Footer
}

const Template = (args) => <Footer {...args} />

export const Basic = Template.bind({})
Basic.args = {
    text: {
        title: 'Footer navigation',
        copyright: 'Crown copyright',
        navMenuAriaLabel: 'Footer legal links'
    },
    navMenuList: [
        {
            isActive: false,
            text: 'Contact us',
            url: '/contact-us'
        },
        {
            isActive: false,
            text: 'Cookies',
            url: '/cookies'
        },
        {
            isActive: false,
            text: 'Privacy policy',
            url: '/privacy-policy'
        },
        {
            isActive: false,
            text: 'Terms and conditions',
            url: '/terms-and-conditions'
        },
    ]
}