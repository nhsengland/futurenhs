import { useState, useEffect } from 'react'
import classNames from 'classnames'

import { crud } from '@constants/crud'
import { SVGIcon } from '@components/SVGIcon'
import { ContentBlock } from '@components/ContentBlock'

import { Props } from './interfaces'

function arraymove(arr, fromIndex, toIndex) {
    var element = arr[fromIndex];
    arr.splice(fromIndex, 1);
    arr.splice(toIndex, 0, element);
}

export const ContentBlockManager: (props: Props) => JSX.Element = ({
    blocks: sourceBlocks,
    templateBlocks,
    currentState = crud.READ,
    stateChangeAction,
    createBlockAction,
    className,
}) => {

    const [mode, setMode] = useState(currentState);
    const [blocks, setBlocks] = useState(sourceBlocks);

    const hasTemplateBlocks: boolean = templateBlocks?.length > 0;
    const hasBlocks: boolean = blocks?.length > 0;
    const isEditable: boolean = mode !== crud.READ;

    const generatedClasses: any = {
        wrapper: classNames(className),
        addBlock: classNames('c-page-manager-block', 'u-text-center'),
        block: classNames('c-page-manager-block'),
        blockHeader: classNames('c-page-manager-block_header', 'u-text-bold'),
        blockBody: classNames('c-page-manager-block_body'),
    }

    const handleCreateBlock = (blockTypeId?: string): void => {
        createBlockAction?.(blockTypeId);
        handleSetToUpdateMode();
    }

    const handleDeleteBlock = (index: number) => {

        const updatedBlocks = [...blocks];

        updatedBlocks.splice(index, 1);

        setBlocks(updatedBlocks);

    }

    const handleMoveBlockPrevious = (currentIndex: number) => {

        const updatedBlocks = [...blocks];

        arraymove(updatedBlocks, currentIndex, currentIndex - 1);

        setBlocks(updatedBlocks);

    }

    const handleMoveBlockNext = (currentIndex: number) => {

        const updatedBlocks = [...blocks];

        arraymove(updatedBlocks, currentIndex, currentIndex + 1);

        setBlocks(updatedBlocks);

    }

    const handleSetToCreateMode = (): void => {
        setMode(crud.CREATE);
        stateChangeAction?.(crud.CREATE)
    }

    const handleSetToUpdateMode = (): void => {
        setMode(crud.UPDATE);
        stateChangeAction?.(crud.UPDATE)
    }

    useEffect(() => {

        setMode(currentState);

    }, [currentState]);

    useEffect(() => {

        setBlocks(sourceBlocks);

    }, [sourceBlocks]);

    return (
        <div className={generatedClasses.wrapper}>
            {(mode === crud.CREATE && hasTemplateBlocks) && (
                <>
                    {hasTemplateBlocks &&
                        <ul className="u-list-none u-p-0">
                            {templateBlocks?.map(({
                                typeId,
                                typeName
                            }, index: number) => {

                                return (

                                    <li key={index}>
                                        <ContentBlock
                                            instanceId={null}
                                            typeId={typeId}
                                            isTemplate={true}
                                            text={{
                                                name: typeName
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
            {(mode === crud.UPDATE || mode === crud.READ) && (
                <>
                    {hasBlocks &&
                        <ul className="u-list-none u-p-0">
                            {blocks?.map(({
                                instanceId,
                                typeId,
                                typeName
                            }, index: number) => {

                                const shouldRenderMovePrevious: boolean = index > 0;
                                const shouldRenderMoveNext: boolean = index < blocks.length - 1;

                                return (

                                    <li key={index}>
                                        <ContentBlock
                                            instanceId={index}
                                            typeId={typeId}
                                            isEditable={isEditable}
                                            shouldRenderMovePrevious={shouldRenderMovePrevious}
                                            shouldRenderMoveNext={shouldRenderMoveNext}
                                            movePreviousAction={handleMoveBlockPrevious}
                                            moveNextAction={handleMoveBlockNext}
                                            deleteAction={handleDeleteBlock}
                                            text={{
                                                name: typeName
                                            }}>
                                                TODO: BLOCK CONTENT
                                        </ContentBlock>
                                    </li>


                                )

                            })}

                        </ul>
                    }
                    {(mode === crud.UPDATE && hasTemplateBlocks) &&
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
