import React, { useState, useEffect, useRef } from 'react'
import classNames from 'classnames'

import { SVGIcon } from '@components/generic/SVGIcon'

import { Props } from './interfaces'
import { CommentLike } from '@services/getGroupDiscussionCommentLikes'

export const Like: (props: Props) => JSX.Element = ({
    targetId,
    iconName = 'icon-like-fill',
    likeCount,
    isLiked,
    shouldEnable,
    likeAction,
    text,
    likes,
    className,
}) => {
    const [isActive, setIsActive] = useState(false)
    const [dynamicLikeCount, setDynamicLikeCount] = useState(likeCount)
    const [isProcessing, setisProcessing] = useState(false)
    const [hasLiked, setHasLiked] = useState(isLiked)
    const names: Array<string> = likes?.map(
        (like) => like.firstRegistered.by.name
    )

    const likeTimeOut = useRef(null)
    const processingTimeOut = useRef(null)

    const { countSingular, countPlural, like, removeLike } = text

    const ariaLabelToUse: string = hasLiked ? removeLike : like

    const generatedClasses: any = {
        wrapper: classNames('c-like', 'u-text-bold', className, {
            ['c-like--enabled']: isActive,
            ['u-text-theme-6']: !shouldEnable,
        }),
        icon: classNames('u-w-5', 'u-h-5', 'u-mr-2', {
            ['u-fill-theme-8']: isActive && hasLiked,
            ['u-fill-theme-5']: !isActive || !hasLiked,
        }),
    }

    const handleLikeToggle = () => {
        if (!isProcessing) {
            window.clearTimeout(likeTimeOut.current)

            const updatedLikeCount: number = hasLiked
                ? dynamicLikeCount - 1
                : dynamicLikeCount + 1

            setHasLiked(!hasLiked)
            setDynamicLikeCount(updatedLikeCount)

            likeTimeOut.current = window.setTimeout(() => {
                const actionToSet: boolean = !hasLiked

                likeAction?.(targetId, actionToSet)
            }, 1000)
        }
    }

    useEffect(() => {
        shouldEnable && setIsActive(true)

        return () => {
            window.clearTimeout(likeTimeOut?.current)
            window.clearTimeout(processingTimeOut?.current)
        }
    }, [])

    useEffect(() => {
        setisProcessing(false)
        setHasLiked(isLiked)
        setDynamicLikeCount(likeCount)

        window.clearTimeout(processingTimeOut?.current)
    }, [isLiked, likeCount])

    if (isActive) {
        return (
            <div>
                <button
                    aria-label={ariaLabelToUse}
                    className={generatedClasses.wrapper}
                    aria-disabled={isProcessing}
                    onClick={handleLikeToggle}
                >
                    <SVGIcon
                        name={iconName}
                        className={generatedClasses.icon}
                    />
                    <span>
                        {dynamicLikeCount}{' '}
                        {dynamicLikeCount === 1 ? countSingular : countPlural}
                    </span>
                </button>
                {names ? <p>Liked by: {names.join(', ')}</p> : null}
            </div>
        )
    }

    return (
        <div>
            <span className={generatedClasses.wrapper}>
                <SVGIcon name={iconName} className={generatedClasses.icon} />
                <span>
                    {dynamicLikeCount}{' '}
                    {dynamicLikeCount === 1 ? countSingular : countPlural}
                </span>
            </span>
            {names ? <p>Liked by: {names}</p> : null}
        </div>
    )
}
