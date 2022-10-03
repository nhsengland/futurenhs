import React from 'react'
import { SVGIcon } from './index'

const iconNames: Array<string> = (window.parent as any).sbIconList || []

export default {
    title: 'SVGIcon',
    component: SVGIcon,
    argTypes: {
        name: {
            options: iconNames,
            control: { type: 'select' },
        },
    },
}

const Template = (args) => <SVGIcon  {...args} />


export const Basic = Template.bind({})
Basic.args = {
    name: iconNames?.[0] || '',
    className: 'u-w-[300px] u-h-[300px] u-fill-theme-0'
}

export const Themed = Template.bind({})
Themed.args = {
    name: iconNames?.[0] || '',
    className: 'u-w-[300px] u-h-[300px] u-fill-theme-8'
}
