import { useMemo } from 'react'
import classNames from 'classnames'

import { Link } from '@components/generic/Link'
import { SVGIcon } from '@components/generic/SVGIcon'
import { capitalise } from '@helpers/formatters/capitalise'

import { Props } from './interfaces'

export const BreadCrumb: (props: Props) => JSX.Element = ({
    breadCrumbList,
    shouldLinkCrumbs = true,
    text,
    seperatorIconName = 'icon-chevron-right',
    truncationMinPathLength,
    truncationStartIndex,
    truncationEndIndex,
    className,
}) => {
    const { ariaLabel, truncation } = text ?? {}

    const hasBreadCrumbElements: boolean = breadCrumbList.length > 0

    const generatedClasses: any = {
        wrapper: classNames('c-breadcrumb', className),
        list: classNames('c-breadcrumb_list'),
        item: classNames('c-breadcrumb_item'),
        link: classNames('c-breadcrumb_link'),
        seperatorIcon: classNames('c-breadcrumb_icon'),
    }

    const isTruncateable: boolean = useMemo(
        () =>
            Number.isInteger(truncationMinPathLength) &&
            breadCrumbList.length > truncationMinPathLength &&
            Number.isInteger(truncationStartIndex) &&
            truncationStartIndex > -1 &&
            Number.isInteger(truncationEndIndex) &&
            truncationEndIndex > truncationStartIndex &&
            truncationEndIndex < breadCrumbList.length,
        [
            truncationStartIndex,
            truncationEndIndex,
            truncationMinPathLength,
            breadCrumbList,
        ]
    )

    let href: string = ''

    return (
        <nav className={generatedClasses.wrapper} aria-label={ariaLabel}>
            {hasBreadCrumbElements && (
                <ol className={generatedClasses.list}>
                    {breadCrumbList.map(({ element, text }, index) => {
                        const isTruncationPoint: boolean =
                            isTruncateable && index === truncationEndIndex - 1
                        const shouldRenderSeperator: boolean =
                            index < breadCrumbList.length - 1
                        const textToUse: string = text
                            ? capitalise({ value: text })
                            : capitalise({ value: element })

                        if (
                            isTruncateable &&
                            !isTruncationPoint &&
                            index >= truncationStartIndex &&
                            index < truncationEndIndex
                        ) {
                            return null
                        }

                        if (shouldLinkCrumbs) {
                            href += href === '/' ? `${element}` : `/${element}`
                        } else {
                            href = element
                        }

                        return (
                            <li key={index} className={generatedClasses.item}>
                                <Link href={href}>
                                    <a className={generatedClasses.link}>
                                        {isTruncationPoint ? (
                                            <>
                                                <span aria-hidden="true">
                                                    {truncation ?? '...'}
                                                </span>
                                                <span className="u-sr-only">
                                                    {textToUse}
                                                </span>
                                            </>
                                        ) : (
                                            textToUse
                                        )}
                                    </a>
                                </Link>
                                {shouldRenderSeperator && (
                                    <SVGIcon
                                        name={seperatorIconName}
                                        className={
                                            generatedClasses.seperatorIcon
                                        }
                                    />
                                )}
                            </li>
                        )
                    })}
                </ol>
            )}
        </nav>
    )
}
