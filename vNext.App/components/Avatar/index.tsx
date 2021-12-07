import React from 'react';

import { Props } from './interfaces';

export const Avatar: (props: Props) => JSX.Element = ({
    initials
}) => {

    return (

        <span className="c-avatar">
            <span className="c-avatar_fallback">
                <span className="c-avatar_initials" aria-hidden="true">{initials}</span>
            </span>
        </span>

    );
}