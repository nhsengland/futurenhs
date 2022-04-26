import { useState, useEffect } from 'react'
import { randomBytes } from 'crypto';
import classNames from 'classnames'

import { formTypes } from '@constants/forms'
import { moveArrayItem, deleteArrayItem } from '@helpers/util/data'
import { cprud } from '@constants/cprud'
import { SVGIcon } from '@components/SVGIcon'
import { Form } from '@components/Form'
import { ContentBlock } from '@components/ContentBlock'
import { TextContentBlock } from '@components/_contentBlockComponents/TextContentBlock';
import { CmsContentBlock } from '@appTypes/contentBlock';

import { Props } from './interfaces'
import { FormErrors } from '@appTypes/form'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';

export const ContentBlockManager: (props: Props) => JSX.Element = ({
    csrfToken,
    forms,
    blocks: sourceBlocks,
    blocksTemplate,
    currentState = cprud.READ,
    blocksChangeAction,
    stateChangeAction,
    createBlockAction,
    className,
}) => {

    const handleInjectUniqueIds = (blocks: Array<CmsContentBlock>): Array<CmsContentBlock> => {

        return blocks.map((block) => {

            block.instanceId = randomBytes(6).toString('hex');

            return block;

        })

    }

    const [mode, setMode] = useState(currentState);
    const [blocks, setBlocks] = useState([]);

    const hasTemplateBlocks: boolean = blocksTemplate?.length > 0;
    const hasBlocks: boolean = blocks?.length > 0;
    const isEditable: boolean = mode !== cprud.READ && mode !== cprud.PREVIEW;

    const generatedClasses: any = {
        wrapper: classNames(className),
        addBlock: classNames('c-page-manager-block', 'u-text-center'),
        block: classNames('c-page-manager-block'),
        blockHeader: classNames('c-page-manager-block_header', 'u-text-bold'),
        blockBody: classNames('c-page-manager-block_body'),
    }

    const handleCreateBlock = (instanceId: string): void => {

        const updatedBlocks = [...blocks];
        const block = JSON.parse(JSON.stringify(blocksTemplate[instanceId]));

        block.instanceId = randomBytes(6).toString('hex');
        updatedBlocks.push(block)

        handleSetToUpdateMode();
        setBlocks(updatedBlocks);
        blocksChangeAction(updatedBlocks);

    }

    const handleDeleteBlock = (instanceId: string): void => {

        const index: number = blocks.findIndex((block) => block.instanceId === instanceId);
        const updatedBlocks = deleteArrayItem(blocks, index);

        setBlocks(updatedBlocks);
        blocksChangeAction(updatedBlocks);

    }

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

    const handleSetToCreateMode = (): void => {
        setMode(cprud.CREATE);
        stateChangeAction?.(cprud.CREATE)
    }

    const handleSetToUpdateMode = (): void => {
        setMode(cprud.UPDATE);
        stateChangeAction?.(cprud.UPDATE)
    }

    const handleUpdateBlockSubmit = async (): Promise<FormErrors> => {

        return {}

    }

    const renderBlockContent = ({ isEditable, block }): JSX.Element => {

        const { instanceId } = block;

        if (block.item.contentType === 'textBlock') {

            if(isEditable){

                const formConfig = JSON.parse(JSON.stringify(forms[formTypes.CONTENT_BLOCK_TEXT]));

                formConfig.initialValues = {
                    ['heading' + '-' + instanceId]: block.content.title,
                    ['mainText' + '-' + instanceId]: block.content.mainText,
                }

                const handleChange = (data) => {}

                return (

                    <LayoutColumnContainer>
                        <LayoutColumn desktop={9}>
                            <Form 
                                key={instanceId}
                                csrfToken={csrfToken}
                                instanceId={instanceId}
                                formConfig={formConfig}
                                shouldRenderSubmitButton={false}
                                changeAction={handleChange}
                                submitAction={handleUpdateBlockSubmit} />
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

    useEffect(() => {

        setMode(currentState);

    }, [currentState]);

    useEffect(() => {

        setBlocks(handleInjectUniqueIds(sourceBlocks));

    }, [sourceBlocks]);

    return (
        <div className={generatedClasses.wrapper}>
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
