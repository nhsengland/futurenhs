import React, { useState } from 'react'
import { Dialog } from './index'

export default {
    title: 'Dialog',
    component: Dialog,
    argTypes: {
        id: {
            control: { type: '' },
        },
        children: {
            control: { type: '' },
        },
        appElement: {
            control: { type: '' },
        },
        confirmAction: {
            control: { type: '' },
        },
        cancelAction: {
            control: { type: '' },
        },
    },
}

const Template = (args) => {
    const [isOpen, setIsOpen] = useState(false)

    const toggleModal = () => {
        setIsOpen(!isOpen)
    }

    return (
        <>
            <button className="c-button" onClick={() => setIsOpen(true)}>
                Open Dialog
            </button>
            <Dialog
                confirmAction={toggleModal}
                cancelAction={toggleModal}
                isOpen={isOpen}
                {...args}
            >
                <h3>Entered Data will be lost</h3>
                <p className="u-text-bold">
                    Any entered details will be discarded. Are you sure you wish
                    to proceed?
                </p>
            </Dialog>
        </>
    )
}

const TemplateConfirmOnly = (args) => {
    const [isOpen, setIsOpen] = useState(false)

    const toggleModal = () => {
        setIsOpen(!isOpen)
    }

    return (
        <>
            <button className="c-button" onClick={() => setIsOpen(true)}>
                Upload file
            </button>
            <Dialog
                confirmAction={toggleModal}
                cancelAction={toggleModal}
                isOpen={isOpen}
                {...args}
            >
                <h3>Upload complete</h3>
                <p className="u-text-bold">
                    The file upload has been successful
                </p>
            </Dialog>
        </>
    )
}

export const Basic = Template.bind({})
Basic.args = {
    id: 'id',
    text: {
        confirmButton: 'Confirm',
        cancelButton: 'Cancel',
    },
}

export const ConfirmOnly = TemplateConfirmOnly.bind({})
ConfirmOnly.args = {
    id: 'id',
    text: {
        confirmButton: 'Ok'
    },
}
