import React from 'react'
import ReactModal from 'react-modal'

import { Props } from './interfaces'

export const Dialog: (props: Props) => JSX.Element = ({
    id,
    isOpen,
    appElement = typeof document !== 'undefined'
        ? document.getElementById('__next')
        : undefined,
    children,
    text,
    confirmAction,
    cancelAction,
}) => {
    const { confirmButton, cancelButton, heading } = text ?? {}

    const handleCancel = () => cancelAction?.()
    const handleConfirm = () => confirmAction?.()

    return (
        <ReactModal
            id={id}
            className="c-dialog"
            background="false"
            isOpen={isOpen}
            appElement={appElement}
            aria={{ labelledby: `dialog-header-${id}` }}
        >
            <div className="c-dialog_content" role="main">
                {heading && (
                    <h1 id={`dialog-header-${id}`} className="nhsuk-heading-l">
                        {heading}
                    </h1>
                )}
                {children}
                {cancelAction && cancelButton && (
                    <button
                        className="c-button c-button--outline u-w-full u-mb-4"
                        onClick={handleCancel}
                    >
                        {cancelButton}
                    </button>
                )}
                {confirmAction && confirmButton && (
                    <button
                        className="c-button u-w-full"
                        onClick={handleConfirm}
                    >
                        {confirmButton}
                    </button>
                )}
            </div>
        </ReactModal>
    )
}
