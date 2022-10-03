import React from 'react'
import { SVGIcon } from '../SVGIcon'
import { Accordion } from './index'

export default {
    title: 'Accordion',
    component: Accordion,
    argTypes: {
        id: {
            control: { type: '' },
        },
        children: {
            control: { type: '' },
        },
        toggleAction: {
            control: { type: '' },
        },
    },
}

const Template = (args) => {
    return (
        <Accordion id={'123'} {...args}>
            <p>Accordion content</p>
        </Accordion>
    )
}

export const Basic = Template.bind({})
Basic.args = {
    toggleClassName: 'u-flex u-items-center',
    contentClassName: 'u-p-4',
    toggleOpenChildren: (
        <>
            <SVGIcon
                name="icon-chevron-up"
                className="u-w-6 u-h-6 u-mr-2 u-fill-theme-0"
            />
            <span className="u-text-2xl">Close accordion</span>
        </>
    ),
    toggleClosedChildren: (
        <>
            <SVGIcon
                name="icon-chevron-down"
                className="u-w-6 u-h-6 u-mr-2 u-fill-theme-0"
            />
            <span className="u-text-2xl">Open accordion</span>
        </>
    ),
}
