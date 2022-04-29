import { useState, useEffect, useRef } from 'react'
import { randomBytes } from 'crypto';
import classNames from 'classnames'

import { themes } from '@constants/themes';
import { formTypes } from '@constants/forms'
import { moveArrayItem, deleteArrayItem } from '@helpers/util/data'
import { cprud } from '@constants/cprud'
import { selectTheme } from '@selectors/themes';
import { SVGIcon } from '@components/SVGIcon'
import { Form } from '@components/Form'
import { ContentBlock } from '@components/ContentBlock'
import { TextContentBlock } from '@components/_contentBlockComponents/TextContentBlock';
import { KeyLinksBlock } from '@components/_contentBlockComponents/KeyLinksBlock';
import { CmsContentBlock } from '@appTypes/contentBlock';
import { RichText } from '@components/RichText';

import { Props } from './interfaces'
import { FormConfig, FormErrors } from '@appTypes/form'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { Theme } from '@appTypes/theme';

const simpleClone = (item) => JSON.parse(JSON.stringify(item));
const simpleCompare = (a: any, b: any): boolean => JSON.stringify(a) === JSON.stringify(b);

/**
 * Generic CMS content block manager
 */
export const ContentBlockManager: (props: Props) => JSX.Element = ({
    csrfToken,
    forms,
    blocks: sourceBlocks,
    blocksTemplate,
    initialState = cprud.READ,
    text,
    shouldRenderEditingHeader,
    blocksChangeAction,
    stateChangeAction,
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

        return simpleClone(sourceBlocks).map((block) => {

            if (shouldAdd) {

                block.instanceId = randomBytes(6).toString('hex');

            } else {

                delete block.instanceId;

            }

            return block;

        })

    }

    const valueUpdateCache: any = useRef({});
    const valueUpdateCacheTimeOut: any = useRef(null);
    const initialBlocks: any = useRef(handleToggleInstanceIds(sourceBlocks, true));

    const [mode, setMode] = useState(initialState);
    const [isJsEnabled, setIsJsEnabled] = useState(false);
    const [referenceBlocks, setReferenceBlocks] = useState(initialBlocks.current);
    const [blocks, setBlocks] = useState(initialBlocks.current);
    const [hasEditedBlocks, setHasEditedBlocks] = useState(false);

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
    const { background }: Theme = selectTheme(themes, themeId);

    const generatedClasses: any = {
        wrapper: classNames(className),
        header: classNames('u-mb-14'),
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
    const EnterUpdateButton: () => JSX.Element = () => <button className={generatedClasses.headerCallOutButton} onClick={handleSetToUpdateMode}>{headerEnterUpdateButton}</button>
    const LeaveUpdateButton: () => JSX.Element = () => <button className={generatedClasses.headerCallOutButton} onClick={handleSetToReadMode}>{headerLeaveUpdateButton}</button>
    const DiscardUpdateButton: () => JSX.Element = () => <button className={generatedClasses.headerCallOutButton} onClick={handleDiscardUpdates}>{headerDiscardUpdateButton}</button>
    const PreviewUpdateButton: () => JSX.Element = () => <button className={generatedClasses.headerCallOutButton} onClick={handleSetToPreviewMode}>{headerPreviewUpdateButton}</button>
    const PublishUpdateButton: () => JSX.Element = () => <button className={generatedClasses.headerPrimaryCallOutButton} onClick={handleUpdateBlockSubmit}>{headerPublishUpdateButton}</button>

    /**
     * Handle creating a new block instance from the page template and adding it to the active block list
     */
    const handleCreateBlock = (instanceId: string): void => {

        const updatedBlocks = [...blocks];
        const block = simpleClone(blocksTemplate[instanceId]);

        block.instanceId = randomBytes(6).toString('hex');
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

        setBlocks(updatedBlocks);
        blocksChangeAction?.(updatedBlocks);

    }

    /**
     * Handle moving a block instance one index backwards in the active block list
     */
    const handleMoveBlockPrevious = (instanceId: string): void => {

        const index: number = blocks.findIndex((block) => block.instanceId === instanceId);
        const targetIndex: number = index - 1;
        const updatedBlocks = moveArrayItem(blocks, index, targetIndex);

        setBlocks(updatedBlocks);
        blocksChangeAction?.(updatedBlocks);

        setTimeout(() => {

            const targetSelector: string = updatedBlocks[targetIndex].instanceId;

            document.getElementById(targetSelector)?.focus()

        }, 0)

    }

    /**
     * Handle moving a block instance one index forwards in the active block list
     */
    const handleMoveBlockNext = (instanceId: string): void => {

        const index: number = blocks.findIndex((block) => block.instanceId === instanceId);
        const targetIndex: number = index + 1;
        const updatedBlocks = moveArrayItem(blocks, index, targetIndex);

        setBlocks(updatedBlocks);
        blocksChangeAction?.(updatedBlocks);

        setTimeout(() => {

            const targetSelector: string = updatedBlocks[targetIndex].instanceId;

            document.getElementById(targetSelector)?.focus()

        }, 0)

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
    const handleSetToPreviewMode = (): void => {
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
    const handleDiscardUpdates = (): void => {
        setBlocks(referenceBlocks);
    }

    /**
     * Handle submitting the current block list data to the API
     */
    const handleUpdateBlockSubmit = async (): Promise<FormErrors> => {

        const blocksToSave: Array<CmsContentBlock> = handleToggleInstanceIds(blocks, false);

        let errors: FormErrors = {};

        if (saveBlocksAction) {

            errors = await saveBlocksAction(blocksToSave);

        }

        if (!Object.keys(errors).length) {

            setMode(cprud.READ);

        }

        return errors
    }

    /**
     * Handle form updates from blocks in edit mode
     */
    const handleFormUpdate = ({ formState, instanceId }): void => {

        const { values = {}, visited } = formState;

        const updatedContent: Record<string, any> = {};
        const targetBlock: CmsContentBlock = blocks.find((block) => block.instanceId === instanceId);

        /**
         * Handle value updates
         */
        Object.keys(values).forEach((key) => {

            const value: any = values[key];
            const fieldName: string = key.replace(`-${instanceId}`, '');

            /**
             * If a field has been interacted with and its current value differs from the value current held in blocks
             */
            if (key.includes(instanceId) && visited[key] && value !== targetBlock.content[fieldName]) {

                updatedContent[fieldName] = values[key];

            }

        });

        /**
         * If there are updates to block content
         */
        if (Object.keys(updatedContent).length > 0) {

            valueUpdateCache.current[instanceId] = Object.assign({}, valueUpdateCache.current[instanceId], updatedContent);

            processValueUpdateCache();

        }

    };

    /**
     * Process the update cache
     * Avoids individual updates to form fields causing full blocks rerender
     */
    const processValueUpdateCache = (): void => {

        window.clearTimeout(valueUpdateCacheTimeOut.current);

        valueUpdateCacheTimeOut.current = window.setTimeout(() => {

            if (valueUpdateCache.current && Object.keys(valueUpdateCache.current).length > 0) {

                const updatedBlocks: Array<CmsContentBlock> = simpleClone(blocks);

                Object.keys(valueUpdateCache.current).forEach((instanceId) => {

                    const targetBlock: CmsContentBlock = updatedBlocks.find((block) => block.instanceId === instanceId);

                    targetBlock.content = Object.assign({}, targetBlock.content, valueUpdateCache.current[instanceId]);

                })

                setBlocks(updatedBlocks);

            }

        }, 250);

    };

    /**
     * Render block data
     */
    const renderBlockContent = ({ isEditable, block }): JSX.Element => {

        const { instanceId, item, content } = block;

        if (item.contentType === 'textBlock') {

            if (isEditable) {

                const formConfig: FormConfig = simpleClone(forms[formTypes.CONTENT_BLOCK_TEXT]);
                const handleChange: (formState: any) => void = (formState) => handleFormUpdate({ instanceId, formState });

                formConfig.initialValues = {
                    [`title-${instanceId}`]: content.title,
                    [`mainText-${instanceId}`]: content.mainText
                };

                return (

                    <LayoutColumnContainer>
                        <LayoutColumn desktop={9}>
                            <Form
                                key={instanceId}
                                csrfToken={csrfToken}
                                instanceId={instanceId}
                                formConfig={formConfig}
                                shouldRenderSubmitButton={false}
                                changeAction={handleChange} />
                        </LayoutColumn>
                    </LayoutColumnContainer>

                )

            }

            return (

                <TextContentBlock
                    headingLevel={3}
                    text={{
                        heading: content.title,
                        bodyHtml: content.mainText
                    }} />

            )

        }

        if (item.contentType === 'keyLinksBlock') {

            if (isEditable) {



            }

            return (

                <KeyLinksBlock
                    headingLevel={3}
                    text={{
                        heading: content.title
                    }} />

            )

        }

    }

    /**
     * On active block data change, compare with source data from API to determine if there are changes
     */
    useEffect(() => {

        const isLocalBlockStateMatchingSource: boolean = simpleCompare(blocks, referenceBlocks);

        if (isLocalBlockStateMatchingSource && hasEditedBlocks) {

            setHasEditedBlocks(false);

        } else if (!isLocalBlockStateMatchingSource && !hasEditedBlocks) {

            setHasEditedBlocks(true);

        }

        return () => window.clearTimeout(valueUpdateCacheTimeOut.current);

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
     * Enable the editing header when JavaScript is enabled
     */
    useEffect(() => {

        setIsJsEnabled(true);

    }, []);

    /**
     * Render
     */
    return (
        <div className={generatedClasses.wrapper}>
            {(isJsEnabled && shouldRenderEditingHeader) &&
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
                                <EnterUpdateButton />
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
                                <EnterUpdateButton />
                                <PublishUpdateButton />
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
                                        <LeaveUpdateButton />
                                    }
                                    {hasEditedBlocks &&
                                        <>
                                            <DiscardUpdateButton />
                                            <PreviewUpdateButton />
                                            <PublishUpdateButton />
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
                                            instanceId={index.toString()}
                                            typeId={item.contentType}
                                            isTemplate={true}
                                            text={{
                                                name: item.name
                                            }}
                                            createAction={handleCreateBlock}>
                                            {renderBlockContent({ block, isEditable: false })}
                                        </ContentBlock>
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
                            {blocks?.map((block, index: number) => {

                                const { instanceId, item } = block;

                                const shouldRenderMovePrevious: boolean = index > 0;
                                const shouldRenderMoveNext: boolean = index < blocks.length - 1;

                                return (

                                    <li key={block.instanceId} className="u-mb-10">
                                        <ContentBlock
                                            instanceId={instanceId}
                                            typeId={item.contentType}
                                            isEditable={isEditable}
                                            shouldRenderMovePrevious={shouldRenderMovePrevious}
                                            shouldRenderMoveNext={shouldRenderMoveNext}
                                            movePreviousAction={handleMoveBlockPrevious}
                                            moveNextAction={handleMoveBlockNext}
                                            deleteAction={handleDeleteBlock}
                                            text={{
                                                name: item.name
                                            }}>
                                            {renderBlockContent({ block, isEditable: mode === cprud.UPDATE })}
                                        </ContentBlock>
                                    </li>


                                )

                            })}

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
        </div>
    )
}
