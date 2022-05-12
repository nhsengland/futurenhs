import { useState, useEffect, useRef } from 'react'
import classNames from 'classnames'
import deepEquals from 'fast-deep-equal';
import FlipMove from 'react-flip-move';

import { useDynamicElementClassName } from '@hooks/useDynamicElementClassName';
import { useTheme } from '@hooks/useTheme';
import { moveArrayItem, deleteArrayItem, simpleClone, hasKeys, createHtmlSafeId } from '@helpers/util/data'
import { cprud } from '@constants/cprud'
import { SVGIcon } from '@components/SVGIcon'
import { Dialog } from '@components/Dialog'
import { ContentBlock } from '@components/ContentBlock'
import { CmsContentBlock } from '@appTypes/contentBlock';
import { RichText } from '@components/RichText';

import { Props } from './interfaces'
import { FormErrors } from '@appTypes/form'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';

/**
 * Generic CMS content block manager
 */
export const ContentBlockManager: (props: Props) => JSX.Element = ({
    blocks: sourceBlocks,
    blocksTemplate,
    initialState = cprud.READ,
    text,
    shouldRenderEditingHeader,
    blocksChangeAction,
    stateChangeAction,
    discardUpdateAction,
    createBlockAction,
    saveBlocksAction,
    themeId,
    className,
}) => {

    /**
     * Blocks in data from the API include no unique IDs so we need to inject some locally
     * to safely manage dynamic block sorting, adding or deleting
     */
    const handleToggleInstanceIds = (sourceBlocks: Array<CmsContentBlock>, shouldAdd: boolean): Array<CmsContentBlock> => {

        if (!sourceBlocks?.length) {

            return [];

        }

        const updatedBlocks: Array<CmsContentBlock> = simpleClone(sourceBlocks)

        const iterator = (blocks: Array<CmsContentBlock>) => {

            blocks.forEach((block) => {

                if (block != null && block.constructor?.name === "Object") {

                    if (shouldAdd) {

                        block.instanceId = createHtmlSafeId();

                    } else {

                        delete block.instanceId;

                    }

                }

                const childBlocks: Array<CmsContentBlock> = block.content?.links; // TODO request links is changed to blocks or children in Umbraco

                if (childBlocks?.length) {

                    iterator(childBlocks);

                }

            })

        }

        iterator(updatedBlocks);

        return updatedBlocks;

    }

    const localErrors: any = useRef({});
    const blockUpdateCache: any = useRef({});
    const blockUpdateCacheTimeOut: any = useRef(null);
    const initialBlocks: any = useRef(handleToggleInstanceIds(sourceBlocks, true));

    const [mode, setMode] = useState(initialState);
    const [referenceBlocks, setReferenceBlocks] = useState(initialBlocks.current);
    const [blocks, setBlocks] = useState(initialBlocks.current);
    const [hasEditedBlocks, setHasEditedBlocks] = useState(false);
    const [isDiscardChangesModalOpen, setIsDiscardChangesModalOpen] = useState(false);
    const [blockIdsInEditMode, setBlockIdsInEditMode] = useState([]);

    const hasTemplateBlocks: boolean = blocksTemplate?.length > 0;
    const hasBlocks: boolean = blocks?.length > 0;
    const isEditable: boolean = mode !== cprud.READ && mode !== cprud.PREVIEW;

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
        headerCallOut: classNames('nhsuk-inset-text u-m-0 u-pr-0 u-max-w-full', `u-border-l-theme-${background}`),
        headerCallOutText: classNames('nhsuk-heading-m u-text-bold'),
        headerCallOutButton: classNames('c-button c-button-outline c-button--min-width u-w-full u-drop-shadow u-mt-4 tablet:u-mt-0 tablet:u-ml-5'),
        headerPrimaryCallOutButton: classNames('c-button c-button--min-width u-w-full u-mt-4 tablet:u-mt-0 tablet:u-ml-5'),
        createBlock: classNames('c-page-manager-block', 'u-text-center'),
        block: classNames('c-page-manager-block'),
        blockHeader: classNames('c-page-manager-block_header', 'u-text-bold'),
        blockBody: classNames('c-page-manager-block_body'),
    }

    /**
     * Action buttons
     */
    const EnterUpdateButton: ({ isDisabled: boolean }) => JSX.Element = ({ isDisabled }) => <button disabled={isDisabled} className={generatedClasses.headerCallOutButton} onClick={handleSetToUpdateMode}>{headerEnterUpdateButton}</button>
    const LeaveUpdateButton: ({ isDisabled: boolean }) => JSX.Element = ({ isDisabled }) => <button disabled={isDisabled} className={generatedClasses.headerCallOutButton} onClick={handleSetToReadMode}>{headerLeaveUpdateButton}</button>
    const DiscardUpdateButton: ({ isDisabled: boolean }) => JSX.Element = ({ isDisabled }) => <button disabled={isDisabled} className={generatedClasses.headerCallOutButton} onClick={handleDiscardUpdates}>{headerDiscardUpdateButton}</button>
    const PreviewUpdateButton: ({ isDisabled: boolean }) => JSX.Element = ({ isDisabled }) => <button disabled={isDisabled} className={generatedClasses.headerCallOutButton} onClick={handleSetToPreviewMode}>{headerPreviewUpdateButton}</button>
    const PublishUpdateButton: ({ isDisabled: boolean }) => JSX.Element = ({ isDisabled }) => <button disabled={isDisabled} className={generatedClasses.headerPrimaryCallOutButton} onClick={handleUpdateBlockSubmit}>{headerPublishUpdateButton}</button>

    /**
     * Handle creating a new block instance from the page template and adding it to the active block list
     */
    const handleCreateBlock = (instanceId: string): void => {

        const updatedBlocks = [...blocks];
        const block = simpleClone(blocksTemplate[instanceId]);

        block.instanceId = createHtmlSafeId();
        updatedBlocks.push(block)

        handleSetToUpdateMode();
        setBlocks(updatedBlocks);
        blocksChangeAction?.(updatedBlocks);

    }

    /**
     * Handle deleting a block instance from the active block list
     */
    const handleDeleteBlock = (instanceId: string): void => {

        const index: number = blocks.findIndex((block) => block.instanceId === instanceId);
        const updatedBlocks = deleteArrayItem(blocks, index);

        if (localErrors.current[instanceId]) {

            delete localErrors.current[instanceId];

        }

        setBlocks(updatedBlocks);
        blocksChangeAction?.(updatedBlocks);

    }

    /**
     * Handle moving a block instance one index backwards in the active block list
     */
    const handleMoveBlockPrevious = (instanceId: string): void => {

        handleMoveBlock(instanceId, -1);

    }

    /**
     * Handle moving a block instance one index forwards in the active block list
     */
    const handleMoveBlockNext = (instanceId: string): void => {

        handleMoveBlock(instanceId, 1);

    }

    /**
     * Handle moving a block instance in the active block list
     */
    const handleMoveBlock = (instanceId: string, offSet: number): void => {

        const index: number = blocks.findIndex((block) => block.instanceId === instanceId);
        const targetIndex: number = index + offSet;
        const updatedBlocks = moveArrayItem(blocks, index, targetIndex);

        setBlockIdsInEditMode([]);

        setTimeout(() => {

            setBlocks(updatedBlocks);

            blocksChangeAction?.(updatedBlocks);

            setTimeout(() => {

                const targetSelector: string = updatedBlocks[targetIndex].instanceId;

                document.getElementById(targetSelector)?.focus()

            }, 0)

        }, 0)

    }

    /**
     * Handle setting an editable block instance to read mode
     */
    const handleSetEditableBlockToReadMode = (instanceId: string): void => {

        if (blockIdsInEditMode.includes(instanceId) && !localErrors.current[instanceId]) {

            const index: number = blockIdsInEditMode.findIndex((item) => item === instanceId);
            const updatedBlockIdsInEditMode: Array<string> = deleteArrayItem(blockIdsInEditMode, index);

            setBlockIdsInEditMode(updatedBlockIdsInEditMode);

        }

    }

    /**
     * Handle setting an editable block instance to update mode
     */
    const handleSetEditableBlockToUpdateMode = (instanceId: string): void => {

        const updatedBlockIdsInEditMode: Array<string> = [...blockIdsInEditMode];

        if (!updatedBlockIdsInEditMode.includes(instanceId)) {

            updatedBlockIdsInEditMode.push(instanceId);
            setBlockIdsInEditMode(updatedBlockIdsInEditMode);

        }

    }

    /**
     * Handle setting the active mode to create
     */
    const handleSetToCreateMode = (): void => {
        setMode(cprud.CREATE);
        stateChangeAction?.(cprud.CREATE)
    }

    /**
     * Handle setting the active mode to read
     */
    const handleSetToReadMode = (): void => {
        setMode(cprud.READ);
        stateChangeAction?.(cprud.READ)
    }

    /**
     * Handle setting the active mode to preview
     */
    const handleSetToPreviewMode = (event): void => {
        setMode(cprud.PREVIEW);
        stateChangeAction?.(cprud.PREVIEW)
    }

    /**
     * Handle setting the active mode to update
     */
    const handleSetToUpdateMode = (): void => {
        setMode(cprud.UPDATE);
        stateChangeAction?.(cprud.UPDATE)
    }

    /**
     * Handle resetting updates to initial state
     */
    const handleDiscardUpdates = (): void => setIsDiscardChangesModalOpen(true);
    const handleDiscardChangesCancel = () => setIsDiscardChangesModalOpen(false);
    const handleDiscardChangesConfirm = () => {

        discardUpdateAction?.();
        setIsDiscardChangesModalOpen(false);
        setMode(cprud.READ);
        setBlocks(referenceBlocks);

        localErrors.current = {};

    }

    /**
     * Handle submitting the current block list data to the API
     */
    const handleUpdateBlockSubmit = async (): Promise<FormErrors> => {

        const blocksToSave: Array<CmsContentBlock> = handleToggleInstanceIds(blocks, false);

        let serverErrors: FormErrors = {};
        let formattedLocalErrors: FormErrors = {};

        if (saveBlocksAction) {

            Object.keys(localErrors.current).forEach((instanceId) => {

                formattedLocalErrors = Object.assign({}, formattedLocalErrors, localErrors.current[instanceId]);

            });

            serverErrors = await saveBlocksAction(blocksToSave, formattedLocalErrors);

        }

        if (!hasKeys(serverErrors)) {

            setMode(cprud.READ);

        }

        return serverErrors
    }

    /**
     * Handle updates from blocks in edit mode
     */
    const handleUpdateBlock = ({ block, instanceId, errors }): void => {

        if (!hasKeys(errors) && localErrors.current.hasOwnProperty(instanceId)) {

            delete localErrors.current[instanceId]

        } else if (hasKeys(errors)) {

            localErrors.current[instanceId] = errors;

        }

        blockUpdateCache.current[instanceId] = Object.assign({}, blockUpdateCache.current[instanceId], block);

        processBlockUpdateCache();

    }

    /**
     * Process the update cache
     * Avoids individual updates to form fields causing full blocks rerender
     */
    const processBlockUpdateCache = (): void => {

        window.clearTimeout(blockUpdateCacheTimeOut.current);

        blockUpdateCacheTimeOut.current = window.setTimeout(() => {

            if (blockUpdateCache.current && hasKeys(blockUpdateCache.current)) {

                const updatedBlocks: Array<CmsContentBlock> = simpleClone(blocks);

                Object.keys(blockUpdateCache.current).forEach((instanceId) => {

                    const targetBlockIndex: number = updatedBlocks.findIndex((block) => block.instanceId === instanceId);

                    updatedBlocks[targetBlockIndex] = blockUpdateCache.current[instanceId];

                })

                blockUpdateCache.current = {};

                setBlocks(updatedBlocks);

            }

        }, 250);

    };

    /**
     * Set non-active blocks to read mode if they have no errors
     */
    const handleLeaveBlock = (event): void => {

        if (mode === cprud.UPDATE && blockIdsInEditMode.length > 0) {

            const updatedBlockIdsInEditMode: Array<string> = [];

            blockIdsInEditMode.forEach((instanceId: string) => {

                const isEventInBlock: boolean = document.getElementById(instanceId)?.contains(event.target);
                const blockHasErrors: boolean = localErrors.current.hasOwnProperty(instanceId);

                if (isEventInBlock || blockHasErrors) {

                    updatedBlockIdsInEditMode.push(instanceId);

                }

            });

            if (!deepEquals(blockIdsInEditMode, updatedBlockIdsInEditMode)) {

                setBlockIdsInEditMode(updatedBlockIdsInEditMode);

            }

        }

    };

    /**
     * Set the class name on main depending on current mode - TODO this would be better in the parent component
     */
    useDynamicElementClassName({
        elementSelector: mode === cprud.READ ? null : 'main',
        addClass: 'u-bg-theme-1',
        removeClass: 'u-bg-theme-3'
    });

    /**
     * On active block data change, compare with source data from API to determine if there are changes
     */
    useEffect(() => {

        const isLocalBlockStateMatchingSource: boolean = deepEquals(blocks, referenceBlocks);

        if (isLocalBlockStateMatchingSource && hasEditedBlocks) {

            setHasEditedBlocks(false);

        } else if (!isLocalBlockStateMatchingSource && !hasEditedBlocks) {

            setHasEditedBlocks(true);

        }

        return () => window.clearTimeout(blockUpdateCacheTimeOut.current);

    }, [blocks]);

    /**
     * Reset the active block data to the API block data if it is updated
     */
    useEffect(() => {

        const updatedBlocks: Array<CmsContentBlock> = handleToggleInstanceIds(sourceBlocks, true);

        setBlocks(updatedBlocks);
        setReferenceBlocks(updatedBlocks);

    }, [sourceBlocks]);

    /**
     * Leave edit mode on click outside
     */
    useEffect(() => {

        document.addEventListener('click', handleLeaveBlock, false);
        document.addEventListener('focus', handleLeaveBlock, false);

        return () => {
            document.removeEventListener('click', handleLeaveBlock, false);
            document.addEventListener('focus', handleLeaveBlock, false);
        }

    }, [mode, blockIdsInEditMode]);

    /**
     * Conditionally reset blocks from edit mode
     */
    useEffect(() => {

        if (blockIdsInEditMode.length > 0 && (mode === cprud.READ || mode === cprud.PREVIEW)) {

            setBlockIdsInEditMode([]);

        }

    }, [mode]);

    /**
     * Render
     */
    return (
        <div className={generatedClasses.wrapper}>
            {shouldRenderEditingHeader &&
                <header className={generatedClasses.header}>
                    {(mode === cprud.READ) &&
                        <LayoutColumnContainer>
                            <LayoutColumn tablet={9}>
                                <div className={generatedClasses.headerCallOut}>
                                    {headerReadBody &&
                                        <RichText bodyHtml={headerReadBody} wrapperElementType="p" className={generatedClasses.headerCallOutText} />
                                    }
                                </div>
                            </LayoutColumn>
                            <LayoutColumn tablet={3} className="u-flex u-items-center">
                                <EnterUpdateButton isDisabled={false} />
                            </LayoutColumn>
                        </LayoutColumnContainer>
                    }
                    {(mode === cprud.PREVIEW) &&
                        <LayoutColumnContainer>
                            <LayoutColumn tablet={6}>
                                <div className={generatedClasses.headerCallOut}>
                                    {headerPreviewBody &&
                                        <RichText bodyHtml={headerPreviewBody} wrapperElementType="p" className={generatedClasses.headerCallOutText} />
                                    }
                                </div>
                            </LayoutColumn>
                            <LayoutColumn tablet={6} className="u-flex u-items-center">
                                <EnterUpdateButton isDisabled={false} />
                                <PublishUpdateButton isDisabled={false} />
                            </LayoutColumn>
                        </LayoutColumnContainer>
                    }
                    {(mode === cprud.CREATE) &&
                        <>
                            {headerCreateHeading &&
                                <h2 className="nhsuk-heading-xl u-mb-8">{headerCreateHeading}</h2>
                            }
                            {headerCreateBody &&
                                <RichText wrapperElementType="p" bodyHtml={headerCreateBody} className="u-text-lead u-text-theme-7" />
                            }
                        </>
                    }
                    {(mode === cprud.UPDATE) &&
                        <div className={generatedClasses.adminCallOut}>
                            <LayoutColumnContainer className="u-mb-6">
                                <LayoutColumn tablet={hasEditedBlocks ? 5 : 9} className="u-flex u-items-center">
                                    {headerUpdateHeading &&
                                        <h2 className="nhsuk-heading-l u-m-0">{headerUpdateHeading}</h2>
                                    }
                                </LayoutColumn>
                                <LayoutColumn tablet={hasEditedBlocks ? 7 : 3} className="tablet:u-flex u-items-center">
                                    {!hasEditedBlocks &&
                                        <LeaveUpdateButton isDisabled={false} />
                                    }
                                    {hasEditedBlocks &&
                                        <>
                                            <DiscardUpdateButton isDisabled={false} />
                                            <PreviewUpdateButton isDisabled={hasKeys(localErrors.current)} />
                                            <PublishUpdateButton isDisabled={false} />
                                        </>
                                    }
                                </LayoutColumn>
                            </LayoutColumnContainer>
                            {headerUpdateBody &&
                                <RichText
                                    wrapperElementType="div"
                                    bodyHtml={headerUpdateBody}
                                    className="u-text-lead u-text-theme-7" />
                            }
                        </div>
                    }
                </header>
            }
            {(mode === cprud.CREATE && hasTemplateBlocks) && (
                <>
                    {hasTemplateBlocks &&
                        <ul className="u-list-none u-p-0">
                            {blocksTemplate?.map((block, index) => {

                                const { item } = block;

                                return (

                                    <li key={index} className="u-mb-10">
                                        <ContentBlock
                                            mode={mode}
                                            instanceId={index.toString()}
                                            typeId={item.contentType}
                                            themeId={themeId}
                                            block={block}
                                            isEditable={true}
                                            text={{
                                                name: item.name
                                            }}
                                            changeAction={handleUpdateBlock}
                                            createAction={handleCreateBlock} />
                                    </li>


                                )

                            })}

                        </ul>
                    }
                    <button
                        onClick={handleSetToUpdateMode}
                        className="c-button c-button-outline u-drop-shadow"
                    >
                        {cancelCreateButton}
                    </button>
                </>
            )}
            {(mode === cprud.UPDATE || mode === cprud.READ || mode === cprud.PREVIEW) && (
                <>
                    {hasBlocks &&
                        <ul className="u-list-none u-p-0">
                            <FlipMove
                                disableAllAnimations={mode !== cprud.UPDATE}
                                enterAnimation="fade"
                                leaveAnimation="fade"
                                duration={200}>
                                {blocks?.map((block: CmsContentBlock, index: number) => {

                                    const { instanceId, item } = block;

                                    const shouldRenderMovePrevious: boolean = index > 0;
                                    const shouldRenderMoveNext: boolean = index < blocks.length - 1;
                                    const isInEditMode: boolean = blockIdsInEditMode.includes(instanceId);
                                    const initialErrors: FormErrors = localErrors.current[instanceId] ?? {};

                                    return (

                                        <li key={index + block.instanceId} className="u-mb-10">
                                            <ContentBlock
                                                mode={mode}
                                                instanceId={instanceId}
                                                typeId={item.contentType}
                                                themeId={themeId}
                                                block={block}
                                                initialErrors={initialErrors}
                                                isEditable={isEditable}
                                                isInEditMode={isInEditMode}
                                                shouldRenderMovePrevious={shouldRenderMovePrevious}
                                                shouldRenderMoveNext={shouldRenderMoveNext}
                                                movePreviousAction={handleMoveBlockPrevious}
                                                moveNextAction={handleMoveBlockNext}
                                                changeAction={handleUpdateBlock}
                                                deleteAction={handleDeleteBlock}
                                                enterReadModeAction={handleSetEditableBlockToReadMode}
                                                enterUpdateModeAction={handleSetEditableBlockToUpdateMode}
                                                text={{
                                                    name: item.name
                                                }} />
                                        </li>


                                    )

                                })}
                            </FlipMove>
                        </ul>
                    }
                    {(mode === cprud.UPDATE && hasTemplateBlocks) &&
                        <div className={generatedClasses.createBlock}>
                            <div className={generatedClasses.blockBody}>
                                <button
                                    onClick={handleSetToCreateMode}
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
                    }
                </>
            )}
            <Dialog
                id="dialog-discard-cms-block-changes"
                isOpen={isDiscardChangesModalOpen}
                text={{
                    cancelButton: 'Cancel',
                    confirmButton: 'Yes, discard',
                }}
                cancelAction={handleDiscardChangesCancel}
                confirmAction={handleDiscardChangesConfirm}
            >
                <h3>Changed Data will be lost</h3>
                <p className="u-text-bold">
                    All changes will be
                    discarded. Are you sure you wish to
                    proceed?
                </p>
            </Dialog>
        </div>
    )
}
