import { useState } from 'react'
import classNames from 'classnames'

import { SVGIcon } from '@components/SVGIcon'
import { Dialog } from '@components/Dialog'

import { Props } from './interfaces'

export const ContentBlock: (props: Props) => JSX.Element = ({
    instanceId,
    typeId,
    isEditable,
    isTemplate,
    shouldRenderMovePrevious,
    shouldRenderMoveNext,
    createAction,
    movePreviousAction,
    moveNextAction,
    deleteAction,
    children,
    text,
    className,
}) => {

    const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false)

    const { name } = text ?? {};

    const isPublished: boolean = !isEditable && !isTemplate;

    const generatedClasses: any = {
        wrapper: classNames('c-content-block', 'focus:u-outline-none', {
            ['c-content-block--editable']: !isPublished
        }, className),
        header: classNames('c-content-block_header'),
        headerButton: classNames('c-content-block_header-button', 'o-link-button', 'u-ml-10'),
        headerButtonIcon: classNames('u-w-6', 'u-h-6', 'u-mr-3'),
        body: classNames('c-content-block_body', {
            ['c-content-block_body--editable']: !isPublished
        }),
    }

    const handleCreate = (event: any): void => createAction?.(instanceId)
    const handleMovePrevious = (event: any): void => movePreviousAction?.(instanceId)
    const handleMoveNext = (event: any): void => moveNextAction?.(instanceId)
    const handleDeleteRequest = (event: any): void => setIsDeleteModalOpen(true)
    const handleDeleteCancel = (index: number) => setIsDeleteModalOpen(false)
    const handleDeleteConfirm = (index: number) => {

        setIsDeleteModalOpen(false);
        deleteAction?.(instanceId)

    }

    return (

        <div id={instanceId} tabIndex={-1} className={generatedClasses.wrapper} data-content-type-id={typeId}>
            {!isPublished &&
                <header className={generatedClasses.header}>
                    <span>{name}</span>
                    <div className="u-flex">
                        {!isTemplate

                            ? <>
                                {shouldRenderMovePrevious &&
                                    <button type="button" onClick={handleMovePrevious} className={generatedClasses.headerButton}>
                                        <SVGIcon name="icon-chevron-up" className={generatedClasses.headerButtonIcon} />
                                        <span>Move block up</span>
                                    </button>
                                }
                                {shouldRenderMoveNext &&
                                    <button type="button" onClick={handleMoveNext} className={generatedClasses.headerButton}>
                                        <SVGIcon name="icon-chevron-down" className={generatedClasses.headerButtonIcon} />
                                        <span>Move block down</span>
                                    </button>
                                }
                                <button type="button" onClick={handleDeleteRequest} className={generatedClasses.headerButton}>
                                    <SVGIcon name="icon-delete" className={generatedClasses.headerButtonIcon} />
                                    <span>Delete</span>
                                </button>
                            </>

                            : <button type="button" onClick={handleCreate} className={generatedClasses.headerButton}>
                                <SVGIcon name="icon-plus-circle" className={generatedClasses.headerButtonIcon} />
                                <span>Add</span>
                            </button>

                        }
                    </div>
                </header>
            }
            <div className={generatedClasses.body}>
                {children}
            </div>
            <Dialog
                id="dialog-discard-cms-block"
                isOpen={isDeleteModalOpen}
                text={{
                    cancelButton: 'Cancel',
                    confirmButton: 'Yes, discard',
                }}
                cancelAction={handleDeleteCancel}
                confirmAction={handleDeleteConfirm}
            >
                <h3>Entered Data will be lost</h3>
                <p className="u-text-bold">
                    Any entered details will be
                    discarded. Are you sure you wish to
                    proceed?
                </p>
            </Dialog>
        </div>

    )
}
