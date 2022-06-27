import classNames from 'classnames'

import { Link } from '@components/Link'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { LayoutWidthContainer } from '@components/LayoutWidthContainer'

import { Props } from './interfaces'

/**
 * Derived from the NHS Design System Footer component: https://service-manual.nhs.uk/design-system/components/footer.
 * Used to show users they are on an NHS service and to help them find links they expect at the bottom of pages.
 */
export const Footer: (props: Props) => JSX.Element = ({
    text,
    navMenuList,
    className,
}) => {
    const { title, navMenuAriaLabel, copyright } = text ?? {}

    const generatedClasses: any = {
        wrapper: classNames('c-site-footer', className),
        heading: classNames('u-sr-only'),
        nav: classNames('c-site-footer_nav'),
        navItem: classNames('c-site-footer_nav-item'),
        copyRight: classNames('c-site-footer_copyright'),
    }

    return (
        <footer className={generatedClasses.wrapper}>
            <h2 className={generatedClasses.heading}>{title}</h2>
            <div className={generatedClasses.nav}>
                <LayoutWidthContainer>
                    <LayoutColumnContainer>
                        <LayoutColumn desktop={10}>
                            <nav aria-label={navMenuAriaLabel}>
                                <ul
                                    role="menu"
                                    className="u-list-none u-m-0 u-p-0"
                                >
                                    {navMenuList.map(
                                        ({ url, text, isActive }, index) => {
                                            return (
                                                <li
                                                    key={index}
                                                    role="none"
                                                    className={
                                                        generatedClasses.navItem
                                                    }
                                                >
                                                    <Link href={url}>
                                                        <a
                                                            role="menuitem"
                                                            rel="noopener"
                                                            aria-current={
                                                                isActive
                                                            }
                                                        >
                                                            {text}
                                                        </a>
                                                    </Link>
                                                </li>
                                            )
                                        }
                                    )}
                                </ul>
                            </nav>
                        </LayoutColumn>
                        {copyright && (
                            <LayoutColumn desktop={2}>
                                <p className={generatedClasses.copyRight}>
                                    <span
                                        dangerouslySetInnerHTML={{
                                            __html: '&copy;',
                                        }}
                                    />{' '}
                                    {copyright}
                                </p>
                            </LayoutColumn>
                        )}
                    </LayoutColumnContainer>
                </LayoutWidthContainer>
            </div>
        </footer>
    )
}
