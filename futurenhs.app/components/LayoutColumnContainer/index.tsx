import React from 'react';
import classNames from 'classnames';

import { Props } from './interfaces';

/**
 * Layout column container
 */
export const LayoutColumnContainer: (props: Props) => JSX.Element = ({
    direction = 'row',
    justify = 'left',
    children,
    className
}) => {

    const generatedClasses: any = {
        container: classNames('l-col-container', className, {
            ['u-flex-column']: direction === 'column',
            ['u-flex-column-reverse']: direction === 'column-reverse',
            ['u-flex-row']: direction === 'row',
            ['u-flex-row-reverse']: direction === 'row-reverse',
            ['u-justify-start']: justify === 'left',
            ['u-justify-center']: justify === 'centre',
            ['u-justify-end']: justify === 'right'
        })
    };

    return (

        <div className={generatedClasses.container}>
            {children}
        </div>

    );

}