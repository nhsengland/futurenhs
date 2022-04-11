import NextLink from 'next/link'

import { Props } from './interfaces'

export const Link: (props: Props) => JSX.Element = ({ href, children }) => {
    const isSameDomain: boolean = true // TODO

    if (isSameDomain) {
        return <NextLink href={href}>{children}</NextLink>
    }

    return children
}
