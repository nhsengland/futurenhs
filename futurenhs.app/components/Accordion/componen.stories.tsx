import React from 'react'
import { Accordion } from './index'

export default {
    title: 'Accordion',
    component: Accordion,
    argTypes: {
        id: {
            control: { type: '' }
        },
        children: {
            control: { type: '' }
        },
        toggleAction: {
            control: { type: '' }
        },
    }
}

const Template = (args) => {
    return (
        <Accordion id={'123'}{...args}>
            <p>Content inside accordion</p>
        </Accordion>
    )
}

export const Basic = Template.bind({})
Basic.args = {
    toggleOpenChildren: 'Close accordion',
    toggleClosedChildren: 'Open accordion'
}