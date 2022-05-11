import { useState, useEffect, useRef } from 'react'
import classNames from 'classnames'

import { simpleClone } from '@helpers/util/data';
import { SVGIcon } from '@components/SVGIcon'
import { Dialog } from '@components/Dialog'
import { TextContentBlock } from '@components/_contentBlockComponents/TextContentBlock';
import { KeyLinksBlock } from '@components/_contentBlockComponents/KeyLinksBlock';

import { Props } from './interfaces'
import { cprud } from '@constants/cprud';

export const ContentBlock: (props: Props) => JSX.Element = ({
    mode,
    instanceId,
    typeId,
    themeId,
    block,
    initialErrors,
    isEditable,
    isInEditMode,
    shouldRenderMovePrevious,
    shouldRenderMoveNext,
    createAction,
    movePreviousAction,
    moveNextAction,
    changeAction,
    deleteAction,
    enterUpdateModeAction,
    text,
    className,
}) => {

    const containerRef = useRef(null);
    const bodyRef = useRef(null);

    const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false)
    const [errors, setErrors] = useState({});

    const { name } = text ?? {};

    const generatedClasses: any = {
        wrapper: classNames('c-content-block', 'focus:u-outline-none', {
            ['c-content-block--editable']: mode === cprud.CREATE || mode === cprud.UPDATE
        }, className),
        header: classNames('c-content-block_header'),
        headerButton: classNames('c-content-block_header-button', 'o-link-button', 'u-ml-10'),
        headerButtonIcon: classNames('u-w-6', 'u-h-6', 'u-mr-3'),
        body: classNames('c-content-block_body', {
            ['c-content-block_body--editable']: mode === cprud.CREATE || mode === cprud.UPDATE
        }),
    }

    const handleSetToUpdateMode = (event: any): void => enterUpdateModeAction?.(instanceId)
    const handleCreate = (event: any): void => createAction?.(instanceId)
    const handleMovePrevious = (event: any): void => movePreviousAction?.(instanceId)
    const handleMoveNext = (event: any): void => moveNextAction?.(instanceId)
    const handleDeleteRequest = (event: any): void => setIsDeleteModalOpen(true)
    const handleDeleteCancel = (index: number) => setIsDeleteModalOpen(false)
    const handleDeleteConfirm = (index: number) => {

        setIsDeleteModalOpen(false);
        deleteAction?.(instanceId)

    }

    const handleBodyClick = () => {

        (isEditable && !isInEditMode) && handleSetToUpdateMode(null);

    }

    /**
     * Handle block value changes
     */
    const handleValueChange = ({ instanceId, formState }) => {

        const { values = {}, errors = {}, visited } = formState;

        const updatedBlock = simpleClone(block);

        let hasChanges: boolean = false;

        /**
         * Handle value updates
         */
        if (block?.content) {

            Object.keys(block.content).forEach((key) => {

                const fieldName: string = `${key}-${instanceId}`
                const value: any = values[fieldName];

                /**
                 * If a field has been interacted with
                 */
                if (visited[fieldName] && updatedBlock.content[key] !== value) {

                    hasChanges = true;
                    updatedBlock.content[key] = value;

                }

            });

        }

        hasChanges && changeAction?.({ instanceId, block: updatedBlock, errors });

    }

    /**
     * Render block content
     */
    const renderBlockContent = (): JSX.Element => {

        const { instanceId, item } = block;

        if (item.contentType === 'textBlock') {

            return (

                <TextContentBlock
                    id={instanceId}
                    isEditable={isInEditMode}
                    headingLevel={3}
                    block={block}
                    initialErrors={initialErrors}
                    changeAction={handleValueChange} />

            )

        }

        if (item.contentType === 'keyLinksBlock') {

            return (

                <KeyLinksBlock
                    id={instanceId}
                    isEditable={isInEditMode}
                    headingLevel={3}
                    block={block}
                    themeId={themeId}
                    initialErrors={initialErrors}
                    changeAction={handleValueChange} />

            )

        }

    }

    return (

        <div
            ref={containerRef}
            id={instanceId}
            tabIndex={-1}
            className={generatedClasses.wrapper} data-content-type-id={typeId}>
            {(isEditable && mode === cprud.CREATE || mode === cprud.UPDATE) &&
                <header className={generatedClasses.header}>
                    <span>{name}</span>
                    <div className="u-flex">
                        {mode === cprud.UPDATE

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
                                {!isInEditMode &&
                                    <button type="button" onClick={handleSetToUpdateMode} className={generatedClasses.headerButton}>
                                        <SVGIcon name="icon-edit" className={generatedClasses.headerButtonIcon} />
                                        <span>Edit</span>
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
            <div ref={bodyRef} className={generatedClasses.body} onClick={handleBodyClick}>
                {renderBlockContent()}
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
