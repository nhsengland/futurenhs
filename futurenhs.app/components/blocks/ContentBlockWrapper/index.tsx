import { useState, useRef } from 'react'
import classNames from 'classnames'

import { cprud } from '@constants/cprud'
import { SVGIcon } from '@components/generic/SVGIcon'
import { Dialog } from '@components/generic/Dialog'

import { Props } from './interfaces'

export const ContentBlockWrapper: (props: Props) => JSX.Element = ({
    mode,
    block,
    isInEditMode,
    shouldRenderMovePrevious,
    shouldRenderMoveNext,
    shouldEnableMovePrevious,
    shouldEnableMoveNext,
    shouldEnableEnterUpdate,
    shouldEnableEnterRead,
    shouldEnableDelete,
    createAction,
    movePreviousAction,
    moveNextAction,
    deleteAction,
    enterUpdateModeAction,
    enterReadModeAction,
    children,
    className,
}) => {
    const containerRef = useRef(null)
    const bodyRef = useRef(null)

    const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false)

    const blockId: string = block.item.id
    const blockContentTypeId: string = block.item.contentType

    const blockLabels: Record<string, string> = {
        textBlock: 'Text Block',
        keyLinksBlock: 'Key Links Block',
    }

    const generatedClasses: any = {
        wrapper: classNames(
            'c-content-block-wrapper',
            'focus:u-outline-none',
            {
                ['c-content-block-wrapper--update']:
                    mode === cprud.CREATE || mode === cprud.UPDATE,
            },
            className
        ),
        header: classNames(
            'c-content-block-wrapper_header',
            'tablet:u-flex',
            'tablet:u-justify-between'
        ),
        headerTitle: classNames('u-block', 'u-mb-5', 'tablet:u-mb-0'),
        headerButton: classNames(
            'c-content-block-wrapper_header-button',
            'o-link-button',
            'u-mt-2',
            'tablet:u-mt-0',
            'tablet:u-ml-10'
        ),
        headerButtonIcon: classNames(
            'u-mr-3',
            'u-w-4',
            'u-h-4',
            'tablet:u-w-6',
            'tablet:u-h-6'
        ),
        body: classNames('c-content-block-wrapper_body', {
            ['c-content-block-wrapper_body--update']:
                mode === cprud.CREATE || mode === cprud.UPDATE,
            ['c-content-block-wrapper_body--updatable']:
                shouldEnableEnterUpdate || mode === cprud.CREATE,
        }),
    }

    const handleSetToUpdateMode = (event: any): void =>
        shouldEnableEnterUpdate && enterUpdateModeAction?.(blockId)
    const handleSetToReadMode = (event: any): void =>
        shouldEnableEnterRead && enterReadModeAction?.(blockId)
    const handleCreate = (event: any): void =>
        createAction?.(blockContentTypeId)
    const handleMovePrevious = (event: any): void =>
        shouldEnableMovePrevious && movePreviousAction?.(blockId)
    const handleMoveNext = (event: any): void =>
        shouldEnableMoveNext && moveNextAction?.(blockId)
    const handleDeleteRequest = (event: any): void => setIsDeleteModalOpen(true)
    const handleDeleteCancel = (index: number) => setIsDeleteModalOpen(false)
    const handleDeleteConfirm = (index: number) => {
        setIsDeleteModalOpen(false)
        deleteAction?.(blockId)
    }

    const handleBodyClick = () => {
        !isInEditMode && mode === cprud.UPDATE && handleSetToUpdateMode(null)
        mode === cprud.CREATE && handleCreate(null)
    }

    return (
        <div
            ref={containerRef}
            id={blockId}
            tabIndex={-1}
            className={generatedClasses.wrapper}
            data-content-type-id={blockContentTypeId}
        >
            {(mode === cprud.CREATE || mode === cprud.UPDATE) && (
                <header className={generatedClasses.header}>
                    <span className={generatedClasses.headerTitle}>
                        {blockLabels[blockContentTypeId]}
                    </span>
                    <div className="tablet:u-flex">
                        {mode === cprud.UPDATE ? (
                            <>
                                {shouldRenderMovePrevious && (
                                    <button
                                        type="button"
                                        disabled={!shouldEnableMovePrevious}
                                        onClick={handleMovePrevious}
                                        className={
                                            generatedClasses.headerButton
                                        }
                                    >
                                        <SVGIcon
                                            name="icon-chevron-up"
                                            className={
                                                generatedClasses.headerButtonIcon
                                            }
                                        />
                                        <span>Move block up</span>
                                    </button>
                                )}
                                {shouldRenderMoveNext && (
                                    <button
                                        type="button"
                                        disabled={!shouldEnableMoveNext}
                                        onClick={handleMoveNext}
                                        className={
                                            generatedClasses.headerButton
                                        }
                                    >
                                        <SVGIcon
                                            name="icon-chevron-down"
                                            className={
                                                generatedClasses.headerButtonIcon
                                            }
                                        />
                                        <span>Move block down</span>
                                    </button>
                                )}
                                {!isInEditMode && (
                                    <button
                                        type="button"
                                        disabled={!shouldEnableEnterUpdate}
                                        onClick={handleSetToUpdateMode}
                                        className={
                                            generatedClasses.headerButton
                                        }
                                    >
                                        <SVGIcon
                                            name="icon-edit"
                                            className={
                                                generatedClasses.headerButtonIcon
                                            }
                                        />
                                        <span>Edit</span>
                                    </button>
                                )}
                                {isInEditMode && (
                                    <button
                                        type="button"
                                        disabled={!shouldEnableEnterRead}
                                        onClick={handleSetToReadMode}
                                        className={
                                            generatedClasses.headerButton
                                        }
                                    >
                                        <SVGIcon
                                            name="icon-tick-circle"
                                            className={
                                                generatedClasses.headerButtonIcon
                                            }
                                        />
                                        <span>Finish editing</span>
                                    </button>
                                )}
                                <button
                                    type="button"
                                    disabled={!shouldEnableDelete}
                                    onClick={handleDeleteRequest}
                                    className={generatedClasses.headerButton}
                                >
                                    <SVGIcon
                                        name="icon-delete"
                                        className={
                                            generatedClasses.headerButtonIcon
                                        }
                                    />
                                    <span>Delete</span>
                                </button>
                            </>
                        ) : (
                            <button
                                type="button"
                                disabled={false}
                                onClick={handleCreate}
                                className={generatedClasses.headerButton}
                            >
                                <SVGIcon
                                    name="icon-plus-circle"
                                    className={
                                        generatedClasses.headerButtonIcon
                                    }
                                />
                                <span>Add</span>
                            </button>
                        )}
                    </div>
                </header>
            )}
            <div
                ref={bodyRef}
                className={generatedClasses.body}
                onClick={handleBodyClick}
            >
                {children}
            </div>
            <Dialog
                id="dialog-discard-cms-block"
                isOpen={isDeleteModalOpen}
                text={{
                    heading: 'Entered Data will be lost',
                    cancelButton: 'Cancel',
                    confirmButton: 'Yes, discard',
                }}
                cancelAction={handleDeleteCancel}
                confirmAction={handleDeleteConfirm}
            >
                <p className="u-text-bold">
                    Any entered details will be discarded. Are you sure you wish
                    to proceed?
                </p>
            </Dialog>
        </div>
    )
}
