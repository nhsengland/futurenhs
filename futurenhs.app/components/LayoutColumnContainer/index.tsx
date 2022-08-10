import React from 'react'
import classNames from 'classnames'

import { Props } from './interfaces'

/**
 * A Flexbox wrapper intended to be composed with child LayoutColumn components to create a responsive grid.
 */
export const LayoutColumnContainer: (props: Props) => JSX.Element = ({
    direction = 'row',
    justify = 'start',
    children,
    className,
}) => {
    const generatedClasses: any = {
        container: classNames('l-col-container', className, {
            ['u-flex-col']: direction === 'col',
            ['u-flex-col-reverse']: direction === 'col-reverse',
            ['u-flex-row']: direction === 'row',
            ['u-flex-row-reverse']: direction === 'row-reverse',
            ['u-justify-start']: justify === 'start',
            ['u-justify-center']: justify === 'centre',
            ['u-justify-end']: justify === 'end',
        }),
    }

    return <div className={generatedClasses.container}>{children}</div>
}
