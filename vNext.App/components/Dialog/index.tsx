import React from 'react';
import ReactModal from 'react-modal';

import { Props } from './interfaces';

export const Dialog: (props: Props) => JSX.Element = ({
    id,
    isOpen,
    appElement = typeof document !== 'undefined' ? document.body : undefined,
    children,
    text,
    confirmAction,
    cancelAction
}) => {

    const { confirmButton, cancelButton } = text ?? {};

    const handleCancel = () => cancelAction?.();
    const handleConfirm = () => confirmAction?.();

    return (

        <ReactModal 
            id={id} 
            className="c-dialog" 
            isOpen={isOpen}
            appElement={appElement}>
                <div className="c-dialog_content">
                    {children}
                    {(cancelAction && cancelButton) &&
                        <button className="c-button c-button--outline c-button--full-width" onClick={handleCancel}>
                            {cancelButton}
                        </button>
                    }
                    {(confirmAction && confirmButton) &&
                        <button className="c-button c-button--full-width" onClick={handleConfirm}>
                            {confirmButton}
                        </button>
                    }
                </div>
        </ReactModal>

    )

}