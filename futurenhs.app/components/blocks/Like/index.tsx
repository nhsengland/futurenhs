import React, { useState, useEffect, useRef } from 'react'
import classNames from 'classnames'

import { SVGIcon } from '@components/generic/SVGIcon'

import { Props } from './interfaces'
import { CommentLike } from '@services/getGroupDiscussionCommentLikes'
import { setDateTimeForTests } from 'public/js/excalidraw/types/utils'

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
    refreshLikes,
    likeIsDisabled,
    openMoreLikes,
    moreLikesIsOpen,
}) => {
    const [isActive, setIsActive] = useState(false)
    const [dynamicLikeCount, setDynamicLikeCount] = useState(likeCount)
    const [isProcessing, setisProcessing] = useState(false)
    const [hasLiked, setHasLiked] = useState(isLiked)
    const [isMoreLikesOpen, setIsMoreLikesOpen] = useState(moreLikesIsOpen)
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
        if (!isProcessing && !likeIsDisabled) {
            window.clearTimeout(likeTimeOut.current)

            const updatedLikeCount: number = hasLiked
                ? dynamicLikeCount - 1
                : dynamicLikeCount + 1

            setHasLiked(!hasLiked)
            setDynamicLikeCount(updatedLikeCount)

            likeTimeOut.current = window.setTimeout(() => {
                const actionToSet: boolean = !hasLiked
                likeAction?.(targetId, actionToSet)
                refreshLikes()
            }, 1000)
        }
    }

    const renderLikes = () => {
        if (Array.isArray(names) && names.length) {
            const likedByString = 'Liked by: '
            const andString = ' and '
            const moreString = ' more'
            const nameList = names.slice(0, 3)
            let text =
                likedByString + nameList.slice(0, -1).join(', ') + andString
            let moreText = names.length - 2 + moreString

            if (names.length <= 3) {
                text =
                    likedByString +
                    nameList.slice(0, -1).join(', ') +
                    andString +
                    nameList.slice(-1)
                moreText = ''
            }

            if (names.length === 1) {
                text = likedByString + nameList[0]
            }
            return (
                <span>
                    {text}
                    <a
                        className="u-cursor-hover"
                        onClick={() => {
                            openMoreLikes(names)
                        }}
                    >
                        {moreText}
                    </a>
                </span>
            )
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
                    disabled={isProcessing}
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

                <p className="nhsuk-body-s u-mt-3 u-text-theme-6">
                    {renderLikes()}
                </p>
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
            <p className="nhsuk-body-s u-mt-3 u-text-theme-6">
                    {renderLikes()}
                </p>
        </div>
    )
}
