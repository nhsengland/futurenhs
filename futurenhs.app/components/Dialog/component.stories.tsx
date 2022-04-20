import React, { useState } from 'react'
import { Dialog } from './index'

export default {
    title: 'Dialog',
    component: Dialog
}

const Template = (args) => {

    const [isOpen, setIsOpen] = useState(false)

    const toggleModal = () => {
        setIsOpen(!isOpen)
    }

    return (
        <>
            <button className="c-button" onClick={() => setIsOpen(true)}>Open Dialog</button>
            <Dialog confirmAction={toggleModal} cancelAction={toggleModal} isOpen={isOpen} {...args} />
        </>
    )
}

export const Basic = Template.bind({})
Basic.args = {
    id: 'id',
    text: {
        confirmButton: 'Confirm',
        cancelButton: 'Cancel'
    },
}