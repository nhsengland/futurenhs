import classNames from 'classnames'

import { Link } from '@components/Link'
import { SVGIcon } from '@components/SVGIcon'

import { Props } from './interfaces'

/**
 * A Next JS router compatible link component, incorporating an icon and accessible aria label.
 */
export const ActionLink: (props: Props) => JSX.Element = ({
    href,
    text,
    iconName,
    className,
}) => {
    const { body, ariaLabel } = text ?? {}

    const generatedClasses: any = {
        wrapper: classNames('u-align-middle', className),
        icon: classNames('u-align-middle', 'u-w-4 u-h-4 u-mr-1 u-fill-theme-0'),
    }

    return (
        <Link href={href}>
            <a className={generatedClasses.wrapper} aria-label={ariaLabel}>
                {iconName && (
                    <SVGIcon
                        name={iconName}
                        className={generatedClasses.icon}
                    />
                )}
                {body}
            </a>
        </Link>
    )
}
