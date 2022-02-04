import React from 'react';

import { Props } from './interfaces';

export const DynamicListContainer: (props: Props) => JSX.Element = ({
    children
}) => {

    return (

        <>
            {children}
        </>

    );
}