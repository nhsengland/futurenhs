import React from 'react';

import { Props } from './interfaces';

export const Avatar: (props: Props) => JSX.Element = ({
    initials
}) => {

    return (

        <span className="c-avatar">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 360.96 358.98" style={{
                fill: 'white',
                color: 'black'
            }}>
                <text style={{
                    fill: 'red',
                    transform: 'translate3d(50%, 50%, 0)',
                    width: '50%',
                    height: '50%',
                    fontSize: '75',
                    }}>SAVE $500</text>
            </svg>
            <span className="c-avatar_fallback">
                <span className="c-avatar_initials" aria-hidden="true">{initials}</span>
            </span>
        </span>

    );
}