import { useState, useEffect } from 'react'
import classNames from 'classnames'

import { moveArrayItem, deleteArrayItem } from '@helpers/util/data'
import { cprud } from '@constants/cprud'
import { SVGIcon } from '@components/SVGIcon'
import { ContentBlock } from '@components/ContentBlock'

import { Props } from './interfaces'

export const ContentBlockManager: (props: Props) => JSX.Element = ({
    blocks: sourceBlocks,
    blocksTemplate,
    currentState = cprud.READ,
    blocksChangeAction,
    stateChangeAction,
    createBlockAction,
    className,
}) => {

    const [mode, setMode] = useState(currentState);
    const [blocks, setBlocks] = useState(sourceBlocks);

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

    const handleCreateBlock = (blockTemplateIndex?: string): void => {

        const updatedBlocks = [...blocks];
        const blockTemplate = blocksTemplate[blockTemplateIndex];

        updatedBlocks.push(blockTemplate)

        handleSetToUpdateMode();
        setBlocks(updatedBlocks);
        blocksChangeAction(updatedBlocks);

    }

    const handleDeleteBlock = (index: number) => {

        const updatedBlocks = deleteArrayItem(blocks, index);

        setBlocks(updatedBlocks);
        blocksChangeAction(updatedBlocks);

    }

    const handleMoveBlockPrevious = (currentIndex: number) => {

        const updatedBlocks = moveArrayItem(blocks, currentIndex, currentIndex - 1);

        setBlocks(updatedBlocks);
        blocksChangeAction(updatedBlocks);

    }

    const handleMoveBlockNext = (currentIndex: number) => {

        const updatedBlocks = moveArrayItem(blocks, currentIndex, currentIndex + 1);

        setBlocks(updatedBlocks);
        blocksChangeAction(updatedBlocks);

    }

    const handleSetToCreateMode = (): void => {
        setMode(cprud.CREATE);
        stateChangeAction?.(cprud.CREATE)
    }

    const handleSetToUpdateMode = (): void => {
        setMode(cprud.UPDATE);
        stateChangeAction?.(cprud.UPDATE)
    }

    useEffect(() => {

        setMode(currentState);

    }, [currentState]);

    useEffect(() => {

        setBlocks(sourceBlocks);

    }, [sourceBlocks]);

    return (
        <div className={generatedClasses.wrapper}>
            {(mode === cprud.CREATE && hasTemplateBlocks) && (
                <>
                    {hasTemplateBlocks &&
                        <ul className="u-list-none u-p-0">
                            {blocksTemplate?.map(({
                                item
                            }, index: number) => {

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
                                                TODO: BLOCK CONTENT
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
                            {blocks?.map(({
                                item
                            }, index: number) => {

                                const shouldRenderMovePrevious: boolean = index > 0;
                                const shouldRenderMoveNext: boolean = index < blocks.length - 1;

                                return (

                                    <li key={index} className="u-mb-10">
                                        <ContentBlock
                                            instanceId={index}
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
                                                TODO: BLOCK CONTENT
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
