import React from 'react'
import classNames from 'classnames'

import { useAssetPath } from '@hooks/useAssetPath';

import { Props } from './interfaces'

export const SVGIcon: (props: Props) => JSX.Element = ({
    url = '/icons/icons.svg',
    name,
    className,
}) => {

    const fullPath: string = useAssetPath(url);
    const xlinkHref: string = fullPath ? `${fullPath}#${name}` : `#${name}`

    const generatedClasses: any = {
        wrapper: classNames('c-svg-icon', className),
    }

    return (
        <svg
            aria-hidden="true"
            role="presentation"
            className={generatedClasses.wrapper}
        >
            <use xlinkHref={xlinkHref} />
        </svg>
    )
}
