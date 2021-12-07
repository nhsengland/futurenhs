import * as React from 'react';
import classNames from 'classnames';

import { Props } from './interfaces';

export const SVGIcon: (props: Props) => JSX.Element = ({
    url = '/icons/icons.svg',
    name, 
    className 
}) => {

    const generatedClasses: any = {
        wrapper: classNames('c-svg-icon', className)
    };

    const xlinkHref: string = url ? `${url}#${name}` : `#${name}`; 

    return (

        <svg aria-hidden="true" role="presentation" className={generatedClasses.wrapper}>
            <use xlinkHref={xlinkHref} />
        </svg>

    );

}