import React from 'react'

import { Props } from './interfaces'

export const AriaLiveRegion: (props: Props) => JSX.Element = ({ children }) => {
    return (
        <div aria-live="polite" aria-atomic="true" aria-relevant="additions">
            {children}
        </div>
    )
}
