import React, { useState, useEffect, useRef } from 'react';
import classNames from 'classnames';

import { SVGIcon } from '@components/SVGIcon';

import { Props } from './interfaces';

export const Like: (props: Props) => JSX.Element = ({
    id,
    iconName = 'icon-like-fill',
    likeCount,
    isLiked,
    likeAction,
    text,
    className
}) => {

    const [isActive, setIsActive] = useState(false);
    const [dynamicLikeCount, setDynamicLikeCount] = useState(likeCount);
    const [hasLiked, setHasLiked] = useState(isLiked);
    const likeTimeOut = useRef(null);

    const { countSingular, countPlural } = text;

    const generatedClasses: any = {
        wrapper: classNames('c-like', className)
    };

    const handleLikeToggle = () => {

        window.clearTimeout(likeTimeOut.current);

        const updatedLikeCount: number = hasLiked ? dynamicLikeCount - 1 : dynamicLikeCount + 1;

        setHasLiked(!hasLiked);
        setDynamicLikeCount(updatedLikeCount);

        likeTimeOut.current = window.setTimeout(() => {

            likeAction?.(id, !hasLiked);

        }, 1500);

    }

    useEffect(() => {

        setIsActive(true);

    }, []);

    useEffect(() => {

        setHasLiked(isLiked);
        setDynamicLikeCount(likeCount);

    }, [isLiked, likeCount]);

    if(isActive){

        return (

            <button className={generatedClasses.wrapper} onClick={handleLikeToggle}>
                <SVGIcon name={iconName} className="u-w-5 u-h-5 u-fill-theme-5" />
                <span>{dynamicLikeCount} {dynamicLikeCount === 1 ? countSingular : countPlural}</span>
            </button>

        )

    }

    return (

        <span className={generatedClasses.wrapper}>
            <SVGIcon name={iconName} className="u-w-5 u-h-5 u-fill-theme-5" />
            <span>{dynamicLikeCount} {dynamicLikeCount === 1 ? countSingular : countPlural}</span>
        </span>

    );
}