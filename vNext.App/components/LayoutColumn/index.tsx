import * as React from 'react';
import classNames from 'classnames';

import { Props } from './interfaces';

/**
 * Layout column
 */
export const LayoutColumn: (props: Props) => JSX.Element = ({
    hasGutters = true,
    mobile = 12,
    tablet,
    desktop,
    className,
    role,
    children
}) => {

    const generatedClasses: any = {
        wrapper: classNames(className, {
            [`l-col-${mobile}`]: hasGutters,
            [`l-col-${tablet}-t`]: hasGutters && Boolean(tablet),
            [`l-col-${desktop}-d`]: hasGutters && Boolean(desktop),
            [`l-col-fb-${mobile}`]: !hasGutters,
            [`l-col-fb-${tablet}-t`]: !hasGutters && Boolean(tablet),
            [`l-col-fb-${desktop}-d`]: !hasGutters && Boolean(desktop),
        })
    };

    return (

        <div className={generatedClasses.wrapper} role={role}>
            {children}
        </div>

    );

}