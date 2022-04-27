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
import { CmsContentBlock } from '@appTypes/contentBlock';
import { RichText } from '@components/RichText';

import { Props } from './interfaces'
import { FormErrors } from '@appTypes/form'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { Theme } from '@appTypes/theme';

/**
 * Generic CMS content block manager
 */
export const ContentBlockManager: (props: Props) => JSX.Element = ({
    csrfToken,
    forms,
    blocks: sourceBlocks,
    blocksTemplate,
    initialState = cprud.READ,
    blocksChangeAction,
    stateChangeAction,
    createBlockAction,
    themeId,
    className,
}) => {

    const currentValues: any = useRef({});

    /**
     * Blocks in data from the API include no unique IDs so we need to inject some locally
     * to safely manage dynamic block sorting, adding or deleting
     */
    const handleInjectUniqueIds = (blocks: Array<CmsContentBlock>): Array<CmsContentBlock> => {

        if(!blocks?.length) {

            return [];

        }

        return blocks.map((block) => {

            block.instanceId = randomBytes(6).toString('hex');

            return block;

        })

    }

    const [mode, setMode] = useState(initialState);
    const [blocks, setBlocks] = useState([]);
    const [hasEditedBlocks, setHasEditedBlocks] = useState(false);

    const hasTemplateBlocks: boolean = blocksTemplate?.length > 0;
    const hasBlocks: boolean = blocks?.length > 0;
    const isEditable: boolean = mode !== cprud.READ && mode !== cprud.PREVIEW;

    const { background }: Theme = selectTheme(themes, themeId);

    const generatedClasses: any = {
        wrapper: classNames(className),
        header: classNames('u-mb-14'),
        headerCallOut: classNames('nhsuk-inset-text u-m-0 u-pr-0 u-max-w-full', `u-border-l-theme-${background}`),
        headerCallOutText: classNames('nhsuk-heading-m u-text-bold'),
        headerCallOutButton: classNames('c-button c-button-outline c-button--min-width u-w-full u-drop-shadow u-mt-4 tablet:u-mt-0 tablet:u-mr-5'),
        headerPrimaryCallOutButton: classNames('c-button c-button--min-width u-w-full u-mt-4 tablet:u-mt-0'),
        addBlock: classNames('c-page-manager-block', 'u-text-center'),
        block: classNames('c-page-manager-block'),
        blockHeader: classNames('c-page-manager-block_header', 'u-text-bold'),
        blockBody: classNames('c-page-manager-block_body'),
    }

    /**
     * Action buttons
     */
    const LeaveEditButton: () => JSX.Element = () => <button className={generatedClasses.headerCallOutButton} onClick={handleSetToReadMode}>Stop editing page</button>
    const EnterEditButton: () => JSX.Element = () => <button className={generatedClasses.headerCallOutButton} onClick={handleSetToUpdateMode}>Edit page</button>
    const DiscardChangesButton: () => JSX.Element = () => <button className={generatedClasses.headerCallOutButton} onClick={handleSetToUpdateMode}>Discard changes</button>
    const PreviewButton: () => JSX.Element = () => <button className={generatedClasses.headerCallOutButton} onClick={handleSetToPreviewMode}>Preview page</button>
    const PublishButton: () => JSX.Element = () => <button className={generatedClasses.headerPrimaryCallOutButton} onClick={handleUpdateBlockSubmit}>Publish group page</button>

    /**
     * Handle creating a new block instance from the page template and adding it to the active block list
     */
    const handleCreateBlock = (instanceId: string): void => {

        const updatedBlocks = [...blocks];
        const block = JSON.parse(JSON.stringify(blocksTemplate[instanceId]));

        block.instanceId = randomBytes(6).toString('hex');
        updatedBlocks.push(block)

        handleSetToUpdateMode();
        setBlocks(updatedBlocks);
        blocksChangeAction(updatedBlocks);

    }

    /**
     * Handle deleting a block instance from the active block list
     */
    const handleDeleteBlock = (instanceId: string): void => {

        const index: number = blocks.findIndex((block) => block.instanceId === instanceId);
        const updatedBlocks = deleteArrayItem(blocks, index);

        setBlocks(updatedBlocks);
        blocksChangeAction(updatedBlocks);

    }

    /**
     * Handle moving a block instance one index backwards in the active block list
     */
    const handleMoveBlockPrevious = (instanceId: string): void => {

        const index: number = blocks.findIndex((block) => block.instanceId === instanceId);
        const targetIndex: number = index - 1;
        const updatedBlocks = moveArrayItem(blocks, index, targetIndex);

        setBlocks(updatedBlocks);
        blocksChangeAction(updatedBlocks);

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
        blocksChangeAction(updatedBlocks);

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
     * Handle submitting the current block list data to the API
     */
    const handleUpdateBlockSubmit = async (): Promise<FormErrors> => {
        setMode(cprud.READ);
        return {}
    }

    /**
     * Render block data
     */
    const renderBlockContent = ({ isEditable, block }): JSX.Element => {

        const { instanceId } = block;

        if (block.item.contentType === 'textBlock') {

            if(isEditable){

                const formConfig = JSON.parse(JSON.stringify(forms[formTypes.CONTENT_BLOCK_TEXT]));
                const headingFieldName: string = 'heading' + '-' + instanceId;
                const mainTextFieldName: string = 'mainText' + '-' + instanceId;

                formConfig.initialValues = {
                    [headingFieldName]: currentValues.current[headingFieldName] || block.content.title,
                    [mainTextFieldName]: currentValues.current[mainTextFieldName] || block.content.mainText
                };

                const handleChange = ({ values }) => {

                    Object.keys(values).forEach((key) => {

                        if(key.includes(instanceId)){

                            currentValues.current[key] = values[key];

                        }

                    });

                    currentValues.current = Object.assign({}, currentValues.current, values);

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
                        heading: block.content.title,
                        bodyHtml: block.content.mainText
                    }} />

            )

        }

    }

    /**
     * On active block data change, compare with source data from API to determine if there are changes
     */
    useEffect(() => {

        const isLocalBlockStateMatchingSource: boolean = JSON.stringify(blocks) == JSON.stringify(sourceBlocks);

        if(isLocalBlockStateMatchingSource && hasEditedBlocks){

            setHasEditedBlocks(false);

        } else if(!isLocalBlockStateMatchingSource && !hasEditedBlocks){

            setHasEditedBlocks(false);

        }

    }, [blocks]);

    /**
     * Reset the active block data to the API block data if it is updated
     */
    useEffect(() => {

        setBlocks(handleInjectUniqueIds(sourceBlocks));

    }, [sourceBlocks]);

    /**
     * Render
     */
    return (
        <div className={generatedClasses.wrapper}>
            <header className={generatedClasses.header}>
                {(mode === cprud.READ) && 
                    <LayoutColumnContainer>
                        <LayoutColumn tablet={9}>
                            <div className={generatedClasses.headerCallOut}>
                                <p className={generatedClasses.headerCallOutText}>You are a Group Admin of this page. Please click edit to switch to editing mode.</p>
                            </div>
                        </LayoutColumn>
                        <LayoutColumn tablet={3} className="u-flex u-items-center">
                            <EnterEditButton />
                        </LayoutColumn>
                    </LayoutColumnContainer>
                }
                {(mode === cprud.PREVIEW) &&
                    <LayoutColumnContainer>
                        <LayoutColumn tablet={6}>
                            <div className={generatedClasses.headerCallOut}>
                                <p className={generatedClasses.headerCallOutText}>You are previewing the group homepage in editing mode.</p>
                            </div>
                        </LayoutColumn>
                        <LayoutColumn tablet={6} className="u-flex u-items-center">
                            <EnterEditButton />
                            <PublishButton />
                        </LayoutColumn>
                    </LayoutColumnContainer>
                }
                {(mode === cprud.CREATE) &&
                    <>
                        <h2 className="nhsuk-heading-xl u-mb-8">Add content block</h2>
                        <RichText wrapperElementType="p" bodyHtml="Choose a content block to add to your group homepage" className="u-text-lead u-text-theme-7" />
                    </>
                }
                {(mode === cprud.UPDATE) &&
                    <div className={generatedClasses.adminCallOut}>
                    <LayoutColumnContainer className="u-mb-6">
                        <LayoutColumn tablet={5} className="u-flex u-items-center">
                            <h2 className="nhsuk-heading-l u-m-0">Editing group homepage</h2>
                        </LayoutColumn>
                        <LayoutColumn tablet={7} className="tablet:u-flex u-items-center">
                            {!hasEditedBlocks && 
                                <LeaveEditButton />
                            }
                            {hasEditedBlocks && 
                                <>
                                    <DiscardChangesButton />
                                    <PreviewButton />
                                    <PublishButton />
                                </>                                  
                            }
                        </LayoutColumn>
                    </LayoutColumnContainer>
                    <RichText
                        wrapperElementType="div"
                        bodyHtml="Welcome to your group homepage. You are currently in editing mode. You can save a draft at any time, preview your page, or publish your changes. Once published, you can edit your page in the group actions. For more information and help, see our quick guide.
    For some inspiration, visit our knowledge hub."
                        className="u-text-lead u-text-theme-7" />
                </div>
                }
            </header>
            {(mode === cprud.CREATE && hasTemplateBlocks) && (
                <>
                    {hasTemplateBlocks &&
                        <ul className="u-list-none u-p-0">
                            {blocksTemplate?.map((block, index) => {

                                const { item } = block;

                                return (

                                    <li key={index} className="u-mb-10">
                                        <ContentBlock
                                            instanceId={index}
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
                        Cancel
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

                                    <li key={block.instanceId} id={instanceId} tabIndex={-1} className="u-mb-10 focus:u-outline-none">
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
                        <div className={generatedClasses.addBlock}>
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
                                        Add content block
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
