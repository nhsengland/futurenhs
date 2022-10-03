import React from 'react'
import classNames from 'classnames'

import { Props } from './interfaces'

/**
 * A Flexbox column intended to be composed with a parent LayoutColumnContainer component to create a responsive grid.
 * Column width configurable across media-query breakpoints. Gutters optional.
 */
export const LayoutColumn: (props: Props) => JSX.Element = ({
    id,
    hasGutters = true,
    mobile = 12,
    tablet,
    desktop,
    className,
    role,
    children,
}) => {
    const generatedClasses: any = {
        wrapper: classNames(className, {
            [`l-col-${mobile}`]: hasGutters && Number(mobile),
            [`l-col-${tablet}-t`]: hasGutters && Number(tablet),
            [`l-col-${desktop}-d`]: hasGutters && Number(desktop),
            [`l-col-fb-${mobile}`]: !hasGutters && Number(mobile),
            [`l-col-fb-${tablet}-t`]: !hasGutters && Number(tablet),
            [`l-col-fb-${desktop}-d`]: !hasGutters && Number(desktop),
        }),
    }

    return (
        <div id={id} className={generatedClasses.wrapper} role={role}>
            {children}
        </div>
    )
}
