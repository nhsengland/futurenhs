import classNames from 'classnames'

import { Link } from '@components/generic/Link'

import { Props } from './interfaces'
import { useEffect, useRef } from 'react'

export const TabbedNav: (props: Props) => JSX.Element = ({
    text,
    navMenuList,
    shouldFocusActiveLink,
}) => {
    const { ariaLabel } = text
    const activeLinkRef = useRef(null)

    const generatedClasses: any = {
        wrapper: classNames('c-tabbed-nav'),
        item: classNames('c-tabbed-nav_item'),
    }

    useEffect(() => {
        if (shouldFocusActiveLink) {
            const activeLink = activeLinkRef.current
            activeLink.setAttribute('tabindex', '-1')
            activeLink.classList.add('focus:u-outline-none')
            activeLink.addEventListener('blur', () => {
                activeLink.removeAttribute('tabindex')
            })
            activeLink.focus()
        }
    }, [activeLinkRef.current])

    return (
        <nav className="c-tabbed-nav_nav" aria-label={ariaLabel}>
            <ul role="menu" className="u-list-plain c-tabbed-nav_list">
                {navMenuList.map(({ url, text, isActive }, index) => {
                    generatedClasses.link = classNames('c-tabbed-nav_link', {
                        ['c-tabbed-nav_link--active u-hidden tablet:u-block']:
                            isActive,
                    })

                    return (
                        <li
                            key={index}
                            role="none"
                            className={generatedClasses.item}
                            ref={isActive ? activeLinkRef : null}
                        >
                            <Link href={url}>
                                <a
                                    role="menuitem"
                                    aria-current={isActive}
                                    className={generatedClasses.link}
                                >
                                    <span>{text}</span>
                                </a>
                            </Link>
                        </li>
                    )
                })}
            </ul>
        </nav>
    )
}
