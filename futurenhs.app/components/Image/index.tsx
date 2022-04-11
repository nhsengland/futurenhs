import React, { useState } from 'react'
import NextImage from 'next/image'

import { Props } from './interfaces'

export const Image: (props: Props) => JSX.Element = ({
    src,
    alt,
    fallBack,
    ...rest
}) => {
    const [imgSrc, setImgSrc] = useState(src)
    const [imgAlt, setImgAlt] = useState(alt)

    const handleError = () => {
        if (fallBack) {
            setImgSrc(fallBack?.src)
            setImgAlt(fallBack?.alt)
        }
    }

    return (
        <NextImage src={imgSrc} alt={imgAlt} onError={handleError} {...rest} />
    )
}
