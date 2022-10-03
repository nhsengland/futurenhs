import React from 'react'

import { SkipLinks } from './index'

export default {
    title: 'SkipLinks',
    component: SkipLinks,
}

const Template = (args) => {
    return (
        <>
            <p>Click here and press tab key to see skip links</p>
            <SkipLinks {...args} />
        </>
    )
}

export const Basic = Template.bind({})
Basic.args = {
    linkList: [
        {
            id: '#main-nav',
            text: 'Skip to main navigation',
        },
        {
            id: '#main',
            text: 'Skip to main content',
        },
    ],
}
