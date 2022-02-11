import React, { useState, useEffect, useRef } from 'react';
import classNames from 'classnames';

import { SVGIcon } from '@components/SVGIcon';

import { Props } from './interfaces';

export const Like: (props: Props) => JSX.Element = ({
    targetId,
    iconName = 'icon-like-fill',
    likeCount,
    isLiked,
    shouldEnable,
    likeAction,
    text,
    className
}) => {

    const [isActive, setIsActive] = useState(false);
    const [dynamicLikeCount, setDynamicLikeCount] = useState(likeCount);
    const [hasLiked, setHasLiked] = useState(isLiked);
    const likeTimeOut = useRef(null);

    const { countSingular, countPlural, like, removeLike } = text;

    const ariaLabelToUse: string = hasLiked ? removeLike : like;

    const generatedClasses: any = {
        wrapper: classNames('c-like', 'u-text-bold', className, {
            ['c-like--enabled']: shouldEnable
        }),
        icon: classNames('u-w-5', 'u-h-5', 'u-mr-2', {
            ['u-fill-theme-8']: hasLiked,
            ['u-fill-theme-5']: !hasLiked
        })
    };

    const handleLikeToggle = () => {

        window.clearTimeout(likeTimeOut.current);

        const updatedLikeCount: number = hasLiked ? dynamicLikeCount - 1 : dynamicLikeCount + 1;

        setHasLiked(!hasLiked);
        setDynamicLikeCount(updatedLikeCount);

        likeTimeOut.current = window.setTimeout(() => {

            likeAction?.(targetId, !hasLiked);

        }, 1000);

    }

    useEffect(() => {

        shouldEnable && setIsActive(true);

    }, []);

    useEffect(() => {

        setHasLiked(isLiked);
        setDynamicLikeCount(likeCount);

    }, [isLiked, likeCount]);

    if(isActive){

        return (

            <button aria-label={ariaLabelToUse} className={generatedClasses.wrapper} onClick={handleLikeToggle}>
                <SVGIcon name={iconName} className={generatedClasses.icon} />
                <span>{dynamicLikeCount} {dynamicLikeCount === 1 ? countSingular : countPlural}</span>
            </button>

        )

    }

    return (

        <span className={generatedClasses.wrapper}>
            <span>{dynamicLikeCount} {dynamicLikeCount === 1 ? countSingular : countPlural}</span>
        </span>

    );
}