import React from 'react'
import { WarningCallout } from './index'

export default {
    title: 'WarningCallout',
    component: WarningCallout,
    argTypes: {},
}

const Template = (args) => <WarningCallout {...args} />

export const Basic = Template.bind({})
Basic.args = {
    headingLevel: 3,
    text: {
        heading: 'JavaScript is disabled',
        body: 'Please enable JavaScript in your browser to see the a preview of this file'
    },
    className: ''
}

