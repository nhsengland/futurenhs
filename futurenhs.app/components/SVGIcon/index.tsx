import React from 'react';
import classNames from 'classnames';

import { Props } from './interfaces';

export const SVGIcon: (props: Props) => JSX.Element = ({
    url = '/icons/icons.svg',
    name, 
    className 
}) => {

    const basePath: string = process.env.NEXT_PUBLIC_ASSET_PREFIX;
    const fullUrl: string = basePath && url ? basePath + url : url ?? '';
    const xlinkHref: string = fullUrl ? `${fullUrl}#${name}` : `#${name}`; 

    const generatedClasses: any = {
        wrapper: classNames('c-svg-icon', className)
    };

    return (

        <svg aria-hidden="true" role="presentation" className={generatedClasses.wrapper}>
            <use xlinkHref={xlinkHref} />
        </svg>

    );

}