import React from 'react'
import { Heading } from './index'

export default {
    title: 'Heading',
    component: Heading,
    argTypes: {
        level: {
            options: [1, 2, 3, 4, 5, 6],
            control: { type: 'radio' },
        },
    },
}

const Template = (args) => <Heading {...args} />

export const Basic = Template.bind({})
Basic.args = {
    level: 1,
    children: 'Heading content',
    className: '',
}
