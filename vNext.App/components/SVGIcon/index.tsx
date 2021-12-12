import * as React from 'react';
import classNames from 'classnames';
import { getEnvVar } from '@helpers/util/env';

import { Props } from './interfaces';

export const SVGIcon: (props: Props) => JSX.Element = ({
    url = '/icons/icons.svg',
    name, 
    className 
}) => {

    const basePath: string = getEnvVar({
        name: 'NEXT_BASE_PATH',
        isRequired: false
    }) as string;

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