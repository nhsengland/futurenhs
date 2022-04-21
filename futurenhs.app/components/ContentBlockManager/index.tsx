import { useState, useEffect } from 'react'
import classNames from 'classnames'

import { crud } from '@constants/crud'
import { SVGIcon } from '@components/SVGIcon'
import { ContentBlock } from '@components/ContentBlock'

import { Props } from './interfaces'

export const ContentBlockManager: (props: Props) => JSX.Element = ({
    blocks,
    currentState = crud.READ,
    stateChangeAction,
    createBlockAction,
    className,
}) => {

    const [mode, setMode] = useState(currentState);

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

    return (
        <div className={generatedClasses.wrapper}>
            {(mode === crud.CREATE) && (
                <>
                    <ul className="u-list-none u-p-0">
                        <li>
                            <ContentBlock
                                instanceId={null}
                                typeId='text'
                                isEditable={true}
                                text={{
                                    name: 'Text block'
                                }}
                                createAction={handleCreateBlock}>
                                <h3 className="nhsuk-heading-l">
                                    Example text block title here
                                </h3>
                                <p>
                                    Here’s an example of a text block and how it
                                    will look on the platform. Lorem ipsum dolor
                                    sit amet, consectetur adipiscing elit, sed
                                    do eiusmod tempor incididunt ut labore et
                                    dolore magna aliqua. Ut enim ad minim
                                    veniam, quis nostrud exercitation ullamco
                                    laboris nisi ut aliquip ex ea commodo
                                    consequat. Duis aute irure dolor in
                                    reprehenderit in voluptate velit esse cillum
                                    dolore eu fugiat nulla pariatur. Excepteur
                                    sint occaecat cupidatat non proident, sunt
                                    in culpa qui officia deserunt mollit anim id
                                    est laborum.
                                </p>
                            </ContentBlock>
                        </li>
                    </ul>
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
                                typeId
                            }, index: number) => {

                                return (

                                    <li key={index}>
                                        <ContentBlock
                                            instanceId={instanceId}
                                            typeId={typeId}
                                            isEditable={isEditable}
                                            text={{
                                                name: ''
                                            }}>
                                            TODO: BLOCK CONTENT
                                        </ContentBlock>
                                    </li>


                                )

                            })}

                        </ul>
                    }
                    {mode === crud.UPDATE &&
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
