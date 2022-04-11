import { useState, useEffect } from 'react'
import { useRouter } from 'next/router'
import classNames from 'classnames'

import { useDynamicElementClassName } from '@hooks/useDynamicElementClassName'
import { actions as actionConstants } from '@constants/actions'
import { themes } from '@constants/themes'
import { selectTheme } from '@selectors/themes'
import { RichText } from '@components/RichText'
import { SVGIcon } from '@components/SVGIcon'
import { LayoutColumn } from '@components/LayoutColumn'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { Theme } from '@appTypes/theme'

import { Props } from './interfaces'

export const PageManager: (props: Props) => JSX.Element = ({
    addBlockAction,
    addBlockCancelAction,
    className,
}) => {
    const [isInAddBlockMode, setIsInAddBlockMode] = useState(false)

    const generatedClasses: any = {
        wrapper: classNames(className),
        addBlock: classNames('c-page-manager-block', 'u-text-center'),
        block: classNames('c-page-manager-block'),
        blockHeader: classNames('c-page-manager-block_header', 'u-text-bold'),
        blockBody: classNames('c-page-manager-block_body'),
    }

    const handleAddBlock = (event: any): void => {
        setIsInAddBlockMode(true)
        addBlockAction?.()
    }

    const handleAddBlockCancel = (event: any): void => {
        setIsInAddBlockMode(false)
        addBlockCancelAction?.()
    }

    return (
        <div className={generatedClasses.wrapper}>
            {isInAddBlockMode && (
                <ul className="u-list-none u-p-0">
                    <li>
                        <div className={generatedClasses.block}>
                            <header className={generatedClasses.blockHeader}>
                                Text block
                            </header>
                            <div className={generatedClasses.blockBody}>
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
                            </div>
                        </div>
                    </li>
                </ul>
            )}
            {isInAddBlockMode ? (
                <button
                    onClick={handleAddBlockCancel}
                    className="c-button c-button-outline u-drop-shadow"
                >
                    Cancel
                </button>
            ) : (
                <div className={generatedClasses.addBlock}>
                    <div className={generatedClasses.blockBody}>
                        <button
                            onClick={handleAddBlock}
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
            )}
        </div>
    )
}
