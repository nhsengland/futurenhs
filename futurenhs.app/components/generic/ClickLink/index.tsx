import classNames from 'classnames'

import { Link } from '@components/generic/Link'
import { SVGIcon } from '@components/generic/SVGIcon'

import { Props } from './interfaces'

/**
 * A Next JS router compatible link component, incorporating an icon and accessible aria label.
 */
export const ClickLink: (props: Props) => JSX.Element = ({
    text,
    iconName,
    material,
    className,
    onClick,
}) => {
    const { body, ariaLabel } = text ?? {}

    const generatedClasses: any = {
        wrapper: classNames('u-align-middle u-cursor-hover', className),
        span: 'u-text-underline o-link-button',
        icon: classNames('u-align-middle', 'u-w-4 u-h-4 u-mr-1 u-fill-theme-8'),
    }

    return (
        <p className={generatedClasses.wrapper} onClick={onClick}>
            <span className={generatedClasses.span}>
                <a aria-label={ariaLabel}>
                    {iconName && (
                        <SVGIcon
                            name={iconName}
                            className={generatedClasses.icon}
                            material
                        />
                    )}
                    {body}
                </a>
            </span>
        </p>
    )
}
