import * as React from 'react';
import classNames from 'classnames';

import { Props } from './interfaces';

/**
 * Layout width container
 */
export const LayoutWidthContainer: (props: Props) => JSX.Element = ({
    children,
    className,
    role 
}) => {

    const generatedClasses: any = {
        container: classNames('l-width-container', className)
    };

    return (

        <div role={role} className={generatedClasses.container}>
            {children}
        </div>

    );

}