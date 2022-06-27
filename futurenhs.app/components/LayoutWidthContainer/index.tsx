import React from 'react'
import classNames from 'classnames'

import { Props } from './interfaces'

/**
 * A wrapper to create a centered content area and restrain child components to a maximum width.
 */
export const LayoutWidthContainer: (props: Props) => JSX.Element = ({
    children,
    className,
    role,
}) => {
    const generatedClasses: any = {
        container: classNames('l-width-container', className),
    }

    return (
        <div role={role} className={generatedClasses.container}>
            {children}
        </div>
    )
}
