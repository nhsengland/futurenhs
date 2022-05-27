import { useState, useEffect, useRef, useCallback } from 'react'
import classNames from 'classnames'
import deepEquals from 'fast-deep-equal'
import FlipMove from 'react-flip-move'

import { useDynamicElementClassName } from '@hooks/useDynamicElementClassName'
import { useTheme } from '@hooks/useTheme'
import {
    moveArrayItem,
    deleteArrayItem,
    simpleClone,
    hasKeys,
} from '@helpers/util/data'
import { cprud } from '@constants/cprud'
import { cmsBlocks } from '@constants/blocks'
import { SVGIcon } from '@components/SVGIcon'
import { Dialog } from '@components/Dialog'
import { ContentBlockWrapper } from '@components/ContentBlockWrapper'
import { CmsContentBlock } from '@appTypes/contentBlock'
import { RichText } from '@components/RichText'
import { TextContentBlock } from '@components/_contentBlockComponents/TextContentBlock'
import { KeyLinksBlock } from '@components/_contentBlockComponents/KeyLinksBlock'

import { Props } from './interfaces'
import { FormErrors } from '@appTypes/form'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'

/**
 * Generic CMS content block manager
 */
export const ContentBlockManager: (props: Props) => JSX.Element = ({
    themeId,
    blocks: sourceBlocks = [],
    blocksTemplate = [],
    initialState = cprud.READ,
    text,
    shouldRenderEditingHeader,
    blocksChangeAction,
    stateChangeAction,
    discardUpdateAction,
    createBlockAction,
    saveBlocksAction,
    className,
}) => {

    const getTypeSafeBlockList: any = (sourceBlocks: Array<CmsContentBlock>): Array<CmsContentBlock> => Array.isArray(sourceBlocks) ? sourceBlocks : [];

    const localErrors: any = useRef({});
    const blockUpdateCache: any = useRef({});
    const blockUpdateCacheTimeOut: any = useRef(null);

    const [mode, setMode] = useState(initialState);
    const [referenceBlocks, setReferenceBlocks] = useState(getTypeSafeBlockList(sourceBlocks));
    const [blocks, setBlocks] = useState(getTypeSafeBlockList(sourceBlocks));
    const [hasEditedBlocks, setHasEditedBlocks] = useState(false);
    const [isDiscardChangesModalOpen, setIsDiscardChangesModalOpen] = useState(false);
    const [blockIdsInEditMode, setBlockIdsInEditMode] = useState([]);

    const hasTemplateBlocks: boolean = blocksTemplate?.length > 0;
    const hasBlocks: boolean = blocks?.length > 0;
    const hasBlockInEditMode: boolean = blockIdsInEditMode.length > 0;

    const { headerReadBody,
            headerPreviewBody,
            headerCreateHeading,
            headerCreateBody,
            headerUpdateHeading,
            headerUpdateBody,
            headerEnterUpdateButton,
            headerLeaveUpdateButton,
            headerDiscardUpdateButton,
            headerPreviewUpdateButton,
            headerPublishUpdateButton,
            createButton,
            cancelCreateButton } = text ?? {};
    const { background } = useTheme(themeId);

    const generatedClasses: any = {
        wrapper: classNames(className),
        header: classNames('u-mb-14', 'u-no-js-hidden'),
        headerCallOut: classNames(
            'nhsuk-inset-text u-m-0 u-pr-0 u-max-w-full',
            `u-border-l-theme-${background}`
        ),
        headerCallOutText: classNames('nhsuk-heading-m u-text-bold'),
        headerCallOutButton: classNames(
            'c-button c-button-outline c-button--min-width u-w-full u-drop-shadow u-mt-4 tablet:u-mt-0 tablet:u-ml-5'
        ),
        headerPrimaryCallOutButton: classNames(
            'c-button c-button--min-width u-w-full u-mt-4 tablet:u-mt-0 tablet:u-ml-5'
        ),
        createBlock: classNames('c-page-manager-block', 'u-text-center'),
        block: classNames('c-page-manager-block'),
        blockHeader: classNames('c-page-manager-block_header', 'u-text-bold'),
        blockBody: classNames('c-page-manager-block_body'),
    }

    /**
     * Action buttons
     */
    const EnterUpdateButton: ({ isDisabled: boolean }) => JSX.Element = ({
        isDisabled,
    }) => (
        <button
            disabled={isDisabled}
            className={generatedClasses.headerCallOutButton}
            onClick={handleSetToUpdateMode}
        >
            {headerEnterUpdateButton}
        </button>
    )
    const LeaveUpdateButton: ({ isDisabled: boolean }) => JSX.Element = ({
        isDisabled,
    }) => (
        <button
            disabled={isDisabled}
            className={generatedClasses.headerCallOutButton}
            onClick={handleSetToReadMode}
        >
            {headerLeaveUpdateButton}
        </button>
    )
    const DiscardUpdateButton: ({ isDisabled: boolean }) => JSX.Element = ({
        isDisabled,
    }) => (
        <button
            disabled={isDisabled}
            className={generatedClasses.headerCallOutButton}
            onClick={handleDiscardUpdates}
        >
            {headerDiscardUpdateButton}
        </button>
    )
    const PreviewUpdateButton: ({ isDisabled: boolean }) => JSX.Element = ({
        isDisabled,
    }) => (
        <button
            disabled={isDisabled}
            className={generatedClasses.headerCallOutButton}
            onClick={handleSetToPreviewMode}
        >
            {headerPreviewUpdateButton}
        </button>
    )
    const PublishUpdateButton: ({ isDisabled: boolean }) => JSX.Element = ({
        isDisabled,
    }) => (
        <button
            disabled={isDisabled}
            className={generatedClasses.headerPrimaryCallOutButton}
            onClick={handleUpdateBlockSubmit}
        >
            {headerPublishUpdateButton}
        </button>
    )

    /**
     * Get all nested block ids
     */
    const getNestedBlockIds = (blockId: string): Array<string> => {
        const ids: Array<string> = [blockId]
        const block: CmsContentBlock = blocks.find(
            (block) => block.item.id === blockId
        )

        block?.content?.blocks?.forEach((block) => ids.push(block.item.id))

        return ids
    }

    /**
     * Get all nested block ids
     */
    const getHasBlockErrors = (blockId: string): boolean => {
        const nestedBlockIds: Array<string> = getNestedBlockIds(blockId)

        let hasErrors: boolean = false

        nestedBlockIds.forEach((blockId) => {
            if (localErrors.current[blockId]) {
                hasErrors = true
            }
        })

        return hasErrors
    }

    /**
     * Handle creating a new block instance from the page template and adding it to the active block list
     */
    const handleCreateBlock = (
        blockContentTypeId: string,
        parentBlockId?: string
    ): void => {
        createBlockAction?.(blockContentTypeId, parentBlockId).then(
            (createdBlockId: string) => {
                const updatedBlocks: Array<CmsContentBlock> = [...blocks]
                const block: CmsContentBlock = simpleClone(
                    blocksTemplate.find(
                        (block) => block.item.contentType === blockContentTypeId
                    )
                )

                block.item.id = createdBlockId
                block.content.blocks = []

                updatedBlocks.push(block)

                handleSetToUpdateMode()
                setBlocks(updatedBlocks)
                setBlockIdsInEditMode([createdBlockId])
                blocksChangeAction?.(updatedBlocks)

                window.setTimeout(() => {
                    document.getElementById(createdBlockId)?.focus()
                }, 0)
            }
        )
    }

    /**
     * Handle deleting a block instance from the active block list
     */
    const handleDeleteBlock = (blockId: string): void => {
        const nestedBlockIds: Array<string> = getNestedBlockIds(blockId)
        const index: number = blocks.findIndex(
            (block) => block.item.id === blockId
        )
        const updatedBlocks = deleteArrayItem(blocks, index)

        nestedBlockIds.forEach((blockId) => {
            if (localErrors.current[blockId]) {
                delete localErrors.current[blockId]
            }
        })

        setBlocks(updatedBlocks)
        blocksChangeAction?.(updatedBlocks)
    }

    /**
     * Handle moving a block instance one index backwards in the active block list
     */
    const handleMoveBlockPrevious = (blockId: string): void => {
        handleMoveBlock(blockId, -1)
    }

    /**
     * Handle moving a block instance one index forwards in the active block list
     */
    const handleMoveBlockNext = (blockId: string): void => {
        handleMoveBlock(blockId, 1)
    }

    /**
     * Handle moving a block instance in the active block list
     */
    const handleMoveBlock = (blockId: string, offSet: number): void => {
        const index: number = blocks.findIndex(
            (block) => block.item.id === blockId
        )
        const targetIndex: number = index + offSet
        const updatedBlocks: Array<CmsContentBlock> = moveArrayItem(
            blocks,
            index,
            targetIndex
        )

        setBlocks(updatedBlocks)

        blocksChangeAction?.(updatedBlocks)

        setTimeout(() => {
            const targetSelector: string = updatedBlocks[targetIndex].item.id

            document.getElementById(targetSelector)?.focus()
        }, 0)
    }

    /**
     * Handle setting an editable block instance to read mode
     */
    const handleSetEditableBlockToReadMode = (blockId: string): void => {
        const hasErrors: boolean = getHasBlockErrors(blockId)

        if (blockIdsInEditMode.includes(blockId) && !hasErrors) {
            const index: number = blockIdsInEditMode.findIndex(
                (id) => id === blockId
            )
            const updatedBlockIdsInEditMode: Array<string> = deleteArrayItem(
                blockIdsInEditMode,
                index
            )

            setBlockIdsInEditMode(updatedBlockIdsInEditMode)
        }
    }

    /**
     * Handle setting an editable block instance to update mode
     */
    const handleSetEditableBlockToUpdateMode = (blockId: string): void => {
        const updatedBlockIdsInEditMode: Array<string> = [...blockIdsInEditMode]

        if (!updatedBlockIdsInEditMode.includes(blockId)) {
            updatedBlockIdsInEditMode.push(blockId)
            setBlockIdsInEditMode(updatedBlockIdsInEditMode)
        }
    }

    /**
     * Handle setting the active mode to create
     */
    const handleSetToCreateMode = useCallback((): void => {
        setMode(cprud.CREATE)
        stateChangeAction?.(cprud.CREATE)
    }, [])

    /**
     * Handle setting the active mode to read
     */
    const handleSetToReadMode = useCallback((): void => {
        setMode(cprud.READ)
        stateChangeAction?.(cprud.READ)
    }, [])

    /**
     * Handle setting the active mode to preview
     */
    const handleSetToPreviewMode = useCallback((event): void => {
        setMode(cprud.PREVIEW)
        stateChangeAction?.(cprud.PREVIEW)
    }, [])

    /**
     * Handle setting the active mode to update
     */
    const handleSetToUpdateMode = useCallback((): void => {
        setMode(cprud.UPDATE)
        stateChangeAction?.(cprud.UPDATE)
    }, [])

    /**
     * Handle resetting updates to initial state
     */
    const handleDiscardUpdates = (): void => setIsDiscardChangesModalOpen(true)
    const handleDiscardChangesCancel = () => setIsDiscardChangesModalOpen(false)
    const handleDiscardChangesConfirm = () => {
        discardUpdateAction?.()
        setIsDiscardChangesModalOpen(false)
        setMode(cprud.READ)
        setBlocks(referenceBlocks)

        localErrors.current = {}
    }

    /**
     * Handle submitting the current block list data to the API
     */
    const handleUpdateBlockSubmit = async (): Promise<FormErrors> => {
        let serverErrors: FormErrors = {}
        let formattedLocalErrors: FormErrors = {}

        if (saveBlocksAction) {
            Object.keys(localErrors.current).forEach((blockId) => {
                formattedLocalErrors = Object.assign(
                    {},
                    formattedLocalErrors,
                    localErrors.current[blockId]
                )
            })

            serverErrors = await saveBlocksAction(blocks, formattedLocalErrors)
        }

        if (!hasKeys(serverErrors)) {
            setMode(cprud.READ)
        }

        return serverErrors
    }

    /**
     * Handle updates from blocks in edit mode
     */
    const handleUpdateBlock = ({ block, errors, childBlockId }): void => {
        const blockId: string = block.item.id
        const errorsId: string = childBlockId ?? blockId

        /**
         * Process the update cache
         * Avoids individual updates to form fields causing full blocks rerender
         */
        window.clearTimeout(blockUpdateCacheTimeOut.current)

        blockUpdateCache.current[blockId] = Object.assign(
            {},
            blockUpdateCache.current[blockId],
            block
        )
        blockUpdateCacheTimeOut.current = window.setTimeout(() => {
            if (
                errors &&
                !hasKeys(errors) &&
                localErrors.current.hasOwnProperty(errorsId)
            ) {
                delete localErrors.current[errorsId]
            } else if (errors && hasKeys(errors)) {
                localErrors.current[errorsId] = errors
            }

            if (blockUpdateCache.current && hasKeys(blockUpdateCache.current)) {
                const updatedBlocks: Array<CmsContentBlock> =
                    simpleClone(blocks)

                Object.keys(blockUpdateCache.current).forEach((blockId) => {
                    const targetBlockIndex: number = updatedBlocks.findIndex(
                        (block) => block.item?.id === blockId
                    )

                    updatedBlocks[targetBlockIndex] =
                        blockUpdateCache.current[blockId]
                })

                blockUpdateCache.current = {}

                setBlocks(updatedBlocks)
            }
        }, 250)
    }

    /**
     * Set non-active blocks to read mode if they have no errors
     */
    const handleDocumentClick = (event): void => {
        let isEventInActiveBlock: boolean = false

        blockIdsInEditMode.forEach((blockId: string) => {
            if (document.getElementById(blockId)?.contains(event.target)) {
                isEventInActiveBlock = true
            }
        })

        if (
            mode === cprud.UPDATE &&
            blockIdsInEditMode.length > 0 &&
            Object.keys(localErrors.current).length === 0 &&
            !isEventInActiveBlock
        ) {
            setBlockIdsInEditMode([])
        }
    }

    /**
     * Render block content
     */
    const renderBlockContent = (block: CmsContentBlock): JSX.Element => {
        const { item } = block
        const { id } = item

        const isInEditMode: boolean = blockIdsInEditMode.includes(id)
        const isInPreviewMode: boolean =
            mode === cprud.CREATE || mode === cprud.UPDATE

        if (item.contentType === cmsBlocks.TEXT) {
            return (
                <TextContentBlock
                    isEditable={isInEditMode}
                    headingLevel={3}
                    block={block}
                    initialErrors={localErrors.current}
                    changeAction={handleUpdateBlock}
                />
            )
        }

        if (item.contentType === cmsBlocks.KEY_LINKS) {
            return (
                <KeyLinksBlock
                    isEditable={isInEditMode}
                    isPreview={isInPreviewMode}
                    headingLevel={3}
                    block={block}
                    themeId={themeId}
                    initialErrors={localErrors.current}
                    createAction={createBlockAction}
                    changeAction={handleUpdateBlock}
                />
            )
        }
    }

    /**
     * Set the class name on main depending on current mode - TODO this would be better in the parent component
     */
    useDynamicElementClassName({
        elementSelector: mode === cprud.READ ? null : 'main',
        addClass: 'u-bg-theme-1',
        removeClass: 'u-bg-theme-3',
    })

    /**
     * On active block data change, compare with source data from API to determine if there are changes
     */
    useEffect(() => {
        const isLocalBlockStateMatchingSource: boolean = deepEquals(
            blocks,
            referenceBlocks
        )

        if (isLocalBlockStateMatchingSource && hasEditedBlocks) {
            setHasEditedBlocks(false)
        } else if (!isLocalBlockStateMatchingSource && !hasEditedBlocks) {
            setHasEditedBlocks(true)
        }

        return () => window.clearTimeout(blockUpdateCacheTimeOut.current)
    }, [blocks])

    /**
     * Reset the active block data to the API block data if it is updated
     */
    useEffect(() => {

        setBlocks(getTypeSafeBlockList(sourceBlocks));
        setReferenceBlocks(getTypeSafeBlockList(sourceBlocks));

    }, [sourceBlocks]);

    /**
     * Leave edit mode on click outside
     */
    useEffect(() => {
        document.addEventListener('click', handleDocumentClick, false)

        return () => {
            document.removeEventListener('click', handleDocumentClick, false)
        }
    }, [mode, blockIdsInEditMode])

    /**
     * Conditionally reset blocks from edit mode
     */
    useEffect(() => {
        if (
            blockIdsInEditMode.length > 0 &&
            (mode === cprud.READ || mode === cprud.PREVIEW)
        ) {
            setBlockIdsInEditMode([])
        }
    }, [mode])

    /**
     * Render
     */
    return (
        <div className={generatedClasses.wrapper}>
            {shouldRenderEditingHeader && (
                <header className={generatedClasses.header}>
                    {mode === cprud.READ && (
                        <LayoutColumnContainer>
                            <LayoutColumn tablet={9}>
                                <div className={generatedClasses.headerCallOut}>
                                    {headerReadBody && (
                                        <h2
                                            className={
                                                generatedClasses.headerCallOutText
                                            }
                                        >
                                            {headerReadBody}
                                        </h2>
                                    )}
                                </div>
                            </LayoutColumn>
                            <LayoutColumn
                                tablet={3}
                                className="u-flex u-items-center"
                            >
                                <EnterUpdateButton isDisabled={false} />
                            </LayoutColumn>
                        </LayoutColumnContainer>
                    )}
                    {mode === cprud.PREVIEW && (
                        <LayoutColumnContainer>
                            <LayoutColumn tablet={6}>
                                <div className={generatedClasses.headerCallOut}>
                                    {headerPreviewBody && (
                                        <h2 className="nhsuk-heading-l u-m-0">
                                            {headerPreviewBody}
                                        </h2>
                                    )}
                                </div>
                            </LayoutColumn>
                            <LayoutColumn
                                tablet={6}
                                className="u-flex u-items-center"
                            >
                                <EnterUpdateButton isDisabled={false} />
                                <PublishUpdateButton isDisabled={false} />
                            </LayoutColumn>
                        </LayoutColumnContainer>
                    )}
                    {mode === cprud.CREATE && (
                        <>
                            {headerCreateHeading && (
                                <h2 className="nhsuk-heading-xl u-mb-8">
                                    {headerCreateHeading}
                                </h2>
                            )}
                            {headerCreateBody && (
                                <RichText
                                    wrapperElementType="p"
                                    bodyHtml={headerCreateBody}
                                    className="u-text-lead u-text-theme-7"
                                />
                            )}
                        </>
                    )}
                    {mode === cprud.UPDATE && (
                        <div className={generatedClasses.adminCallOut}>
                            <LayoutColumnContainer className="u-mb-6">
                                <LayoutColumn
                                    tablet={hasEditedBlocks ? 5 : 9}
                                    className="u-flex u-items-center"
                                >
                                    {headerUpdateHeading && (
                                        <h2 className="nhsuk-heading-l u-m-0">
                                            {headerUpdateHeading}
                                        </h2>
                                    )}
                                </LayoutColumn>
                                <LayoutColumn
                                    tablet={hasEditedBlocks ? 7 : 3}
                                    className="tablet:u-flex u-items-center"
                                >
                                    {!hasEditedBlocks && (
                                        <LeaveUpdateButton isDisabled={false} />
                                    )}
                                    {hasEditedBlocks && (
                                        <>
                                            <DiscardUpdateButton
                                                isDisabled={false}
                                            />
                                            <PreviewUpdateButton
                                                isDisabled={hasKeys(
                                                    localErrors.current
                                                )}
                                            />
                                            <PublishUpdateButton
                                                isDisabled={false}
                                            />
                                        </>
                                    )}
                                </LayoutColumn>
                            </LayoutColumnContainer>
                            {headerUpdateBody && (
                                <RichText
                                    wrapperElementType="div"
                                    bodyHtml={headerUpdateBody}
                                    className="u-text-lead u-text-theme-7"
                                />
                            )}
                        </div>
                    )}
                </header>
            )}
            {mode === cprud.CREATE && hasTemplateBlocks && (
                <>
                    {hasTemplateBlocks && (
                        <ul className="u-list-none u-p-0">
                            {blocksTemplate?.map((block, index) => {
                                return (
                                    <li key={index} className="u-mb-10">
                                        <ContentBlockWrapper
                                            mode={mode}
                                            block={block}
                                            createAction={handleCreateBlock}
                                        >
                                            {renderBlockContent(block)}
                                        </ContentBlockWrapper>
                                    </li>
                                )
                            })}
                        </ul>
                    )}
                    <button
                        onClick={handleSetToUpdateMode}
                        className="c-button c-button-outline u-drop-shadow"
                    >
                        {cancelCreateButton}
                    </button>
                </>
            )}
            {(mode === cprud.UPDATE ||
                mode === cprud.READ ||
                mode === cprud.PREVIEW) && (
                <>
                    {hasBlocks && (
                        <ul className="u-list-none u-p-0 u-relative">
                            <FlipMove
                                typeName={null}
                                disableAllAnimations={mode !== cprud.UPDATE}
                                enterAnimation="fade"
                                leaveAnimation="fade"
                                duration={100}
                            >
                                {blocks?.map(
                                    (block: CmsContentBlock, index: number) => {
                                        const blockId: string = block.item.id

                                        const key: string = index + blockId
                                        const shouldRenderMovePrevious: boolean =
                                            index > 0
                                        const shouldRenderMoveNext: boolean =
                                            index < blocks.length - 1
                                        const isInEditMode: boolean =
                                            blockIdsInEditMode.includes(blockId)
                                        const isEditable: boolean =
                                            mode !== cprud.READ &&
                                            mode !== cprud.PREVIEW
                                        const hasErrors: boolean =
                                            getHasBlockErrors(blockId)

                                        return (
                                            <li key={key} className="u-mb-10">
                                                {isEditable ? (
                                                    <ContentBlockWrapper
                                                        key={key}
                                                        mode={mode}
                                                        block={block}
                                                        isInEditMode={
                                                            isInEditMode
                                                        }
                                                        shouldRenderMovePrevious={
                                                            shouldRenderMovePrevious
                                                        }
                                                        shouldRenderMoveNext={
                                                            shouldRenderMoveNext
                                                        }
                                                        shouldEnableMovePrevious={
                                                            !hasBlockInEditMode
                                                        }
                                                        shouldEnableMoveNext={
                                                            !hasBlockInEditMode
                                                        }
                                                        shouldEnableDelete={
                                                            !hasBlockInEditMode ||
                                                            isInEditMode
                                                        }
                                                        shouldEnableEnterUpdate={
                                                            !hasBlockInEditMode
                                                        }
                                                        shouldEnableEnterRead={
                                                            isInEditMode &&
                                                            !hasErrors
                                                        }
                                                        movePreviousAction={
                                                            handleMoveBlockPrevious
                                                        }
                                                        moveNextAction={
                                                            handleMoveBlockNext
                                                        }
                                                        deleteAction={
                                                            handleDeleteBlock
                                                        }
                                                        enterReadModeAction={
                                                            handleSetEditableBlockToReadMode
                                                        }
                                                        enterUpdateModeAction={
                                                            handleSetEditableBlockToUpdateMode
                                                        }
                                                    >
                                                        {renderBlockContent(
                                                            block
                                                        )}
                                                    </ContentBlockWrapper>
                                                ) : (
                                                    renderBlockContent(block)
                                                )}
                                            </li>
                                        )
                                    }
                                )}
                            </FlipMove>
                        </ul>
                    )}
                    {mode === cprud.UPDATE && hasTemplateBlocks && (
                        <div className={generatedClasses.createBlock}>
                            <div className={generatedClasses.blockBody}>
                                <button
                                    onClick={handleSetToCreateMode}
                                    disabled={hasBlockInEditMode}
                                    className="c-button c-button-outline u-drop-shadow"
                                >
                                    <SVGIcon
                                        name="icon-add-content"
                                        className="u-w-9 u-h-8 u-mr-4 u-align-middle"
                                    />
                                    <span className="u-align-middle">
                                        {createButton}
                                    </span>
                                </button>
                            </div>
                        </div>
                    )}
                </>
            )}
            <Dialog
                id="dialog-discard-cms-block-changes"
                isOpen={isDiscardChangesModalOpen}
                text={{
                    cancelButton: 'Cancel',
                    confirmButton: 'Yes, discard',
                    heading: 'Changed Data will be lost',
                }}
                cancelAction={handleDiscardChangesCancel}
                confirmAction={handleDiscardChangesConfirm}
            >
                <p className="u-text-bold">
                    All changes will be discarded. Are you sure you wish to
                    proceed?
                </p>
            </Dialog>
        </div>
    )
}
