import React, { useEffect, useState } from 'react';
import classNames from 'classnames';

import { Props } from './interfaces';

export const RemainingCharacterCount: Function = ({ 
    id,
    currentCharacterCount,
    maxCharacterCount,
    remainingCharactersText = 'characters remaining',
    remainingCharactersExceededText = 'characters too many',
    className 
}: Props): JSX.Element => {

    const [shouldRender, setShouldRender] = useState(false);

    const displayNumber: number = maxCharacterCount - currentCharacterCount;
    const message: string = (displayNumber < 0) ? remainingCharactersExceededText : remainingCharactersText;
    const isOverMaxCharacterCount: boolean = displayNumber < 0;
    const countMessage: string = `${Math.abs(displayNumber)} ${message}`;

    const generatedClasses: any = {
        wrapper: classNames('nhsuk-hint', 'u-mt-1', 'u-mb-0', className)
    };

    /**
     * Render the dynamic version of the component in the client environment
     */
    useEffect(() => {

        setShouldRender(true);

    }, []);

    if(!shouldRender){

        return null;

    }

    return (

        <span id={id} aria-live="polite" className={generatedClasses.wrapper}>
            {countMessage}
        </span>

    )

}
