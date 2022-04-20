import { useState, useEffect } from 'react'
import { useRouter } from 'next/router'
import classNames from 'classnames'

import { themes } from '@constants/themes'
import { selectTheme } from '@selectors/themes'
import { RichText } from '@components/RichText'
import { SVGIcon } from '@components/SVGIcon'
import { LayoutColumn } from '@components/LayoutColumn'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { Theme } from '@appTypes/theme'

import { Props } from './interfaces'

export const ContentBlock: (props: Props) => JSX.Element = ({
    blockTypeId,
    blockInstanceId,
    isEditable,
    shouldRenderMovePrevious,
    shouldRenderMoveNext,
    createAction,
    movePreviousAction,
    moveNextAction,
    deleteAction,
    children,
    text,
    className,
}) => {

    const { name } = text ?? {};

    const shouldRenderHeader: boolean = !blockInstanceId || isEditable;

    const generatedClasses: any = {
        wrapper: classNames('c-content-block', className),
        header: classNames('c-content-block_header'),
        headerButton: classNames('c-content-block_header-button', 'o-link-button', 'u-ml-10'),
        headerButtonIcon: classNames('u-w-6', 'u-h-6', 'u-mr-3'),
        body: classNames('c-content-block_body'),
    }

    const handleCreate = (event: any): void => {

        event.preventDefault();

        createAction?.(blockTypeId)
    }

    const handleMovePrevious = (event: any): void => {

        event.preventDefault();

        movePreviousAction?.(blockInstanceId)
    }

    const handleMoveNext = (event: any): void => {

        event.preventDefault();

        moveNextAction?.(blockInstanceId)
    }

    const handleDelete = (event: any): void => {

        event.preventDefault();

        deleteAction?.(blockInstanceId)
    }

    return (

        <div className={generatedClasses.wrapper}>
            {shouldRenderHeader &&
                <header className={generatedClasses.header}>
                    <span>{name}</span>
                    <div className="u-flex">
                        {blockInstanceId
                        
                            ?   <>
                                    {shouldRenderMovePrevious &&
                                        <button onClick={handleMovePrevious} className={generatedClasses.headerButton}>
                                            <SVGIcon name="icon-chevron-up" className={generatedClasses.headerButtonIcon} />
                                            <span>Move block up</span>
                                        </button>
                                    }
                                    {shouldRenderMoveNext &&
                                        <button onClick={handleMoveNext} className={generatedClasses.headerButton}>
                                            <SVGIcon name="icon-chevron-down" className={generatedClasses.headerButtonIcon} />
                                            <span>Move block down</span>
                                        </button>
                                    }
                                    <button onClick={handleDelete} className={generatedClasses.headerButton}>
                                        <SVGIcon name="icon-delete" className={generatedClasses.headerButtonIcon} />
                                        <span>Delete</span>
                                    </button>
                                </>
    
                            :   <button onClick={handleCreate} className={generatedClasses.headerButton}>
                                    <SVGIcon name="icon-plus-circle" className={generatedClasses.headerButtonIcon} />
                                    <span>Add</span>
                                </button>
    
                        }
                    </div>
                </header>
            }
            <div className={generatedClasses.body}>
                {children}
            </div>
        </div>

    )
}
