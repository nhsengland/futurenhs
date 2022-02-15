import React from 'react';
import classNames from 'classnames';
import Image from 'next/image';

import { Props } from './interfaces';

export const Card: (props: Props) => JSX.Element = ({
    image,
    children,
    className
}) => {

    const { src, altText, height, width } = image ?? {};

    const generatedClasses: any = {
        wrapper: classNames('c-card', className),
        hero: classNames('c-card_hero'),
        heroBody: classNames('c-card_hero-body'),
        body: classNames('c-card_body')
    };

    return (

        <div className={generatedClasses.wrapper}>
            {image &&
                <div className={generatedClasses.hero}>
                    <div className={generatedClasses.heroBody}>
                        <Image 
                            src={src} 
                            alt={altText} 
                            height={height} 
                            width={width} />
                    </div>
                </div>
            }
            {children}
        </div>

    );
}