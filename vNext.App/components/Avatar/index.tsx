import React from 'react';
import classNames from 'classnames';

import { Image } from '@components/Image';

import { Props } from './interfaces';

export const Avatar: (props: Props) => JSX.Element = ({
    image,
    initials,
    className
}) => {

    const { src, 
            height, 
            width, 
            altText } = image ?? {};

    const generatedClasses: any = {
        wrapper: classNames('c-avatar', className),
        image: classNames('c-avatar_image'),
        fallBack: classNames('c-avatar_fallback', 'u-min-w-fit'),
        fallBackInitials: classNames('c-avatar_fallback-initials')
    };

    return (

        <span className={generatedClasses.wrapper}>

            {image 
            
                ?   <Image 
                        src={src}
                        height={height}
                        width={width}
                        alt={altText} />

                :   <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 360.96 358.98" className={generatedClasses.fallBack}>
                        <text 
                            x="50%" 
                            y="50%"
                            dominantBaseline="central" 
                            textAnchor="middle"
                            className={generatedClasses.fallBackInitials}>
                                {initials}
                        </text>
                    </svg> 

            }

        </span>

    );
}