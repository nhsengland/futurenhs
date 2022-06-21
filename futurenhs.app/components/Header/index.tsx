import { useState, useEffect } from 'react'
import { useRouter } from 'next/router'
import classNames from 'classnames'

import { actions as actionTypes } from '@constants/actions'
import { initials } from '@helpers/formatters/initials'
import { Image } from '@components/Image'
import { Link } from '@components/Link'
import { LayoutWidthContainer } from '@components/LayoutWidthContainer'
import { ErrorBoundary } from '@components/ErrorBoundary'
import { Search } from '@components/Search'
import { SVGIcon } from '@components/SVGIcon'
import { SkipLinks } from '@components/SkipLinks'
import { Accordion } from '@components/Accordion'
import { Dialog } from '@components/Dialog'
import { Avatar } from '@components/Avatar'
import { useMediaQuery } from '@hooks/useMediaQuery'
import * as domHelpers from '@helpers/dom'
import { mediaQueries } from '@constants/css'
import { iconNames } from '@constants/icons'
import { requestMethods } from '@constants/fetch'
import { routes } from '@constants/routes'
import { capitalise } from '@helpers/formatters/capitalise'

import { Props } from './interfaces'
import { useAssetPath } from '@hooks/useAssetPath'
import { Image as ImageType } from '@appTypes/image'

/**
 * Site header
 */
export const Header: (props: Props) => JSX.Element = ({
    skipLinkList = [
        {
            id: '#main-nav',
            text: 'Skip to main navigation',
        },
        {
            id: '#main',
            text: 'Skip to main content',
        },
    ],
    shouldRenderSearch = true,
    shouldRenderNavigation = true,
    navMenuList,
    text,
    user,
    actions,
    searchTerm,
}) => {
    const router = useRouter()

    const [isMainAccordionOpen, setIsMainAccordionOpen] = useState(true)
    const [isLogoutModalOpen, setIsLogoutModalOpen] = useState(false)
    const [isUserAccordionOpen, setIsUserAccordionOpen] = useState(false)

    const { text: userText } = user ?? {}
    const { userName } = userText ?? {}
    const userInitials: string = initials({ value: userName })

    const {
        editProfile,
        admin,
        logOut,
        logOutHeading,
        logOutBody,
        logOutCancel,
        logOutConfirm,
    } = text ?? {}

    const isDesktop: boolean = useMediaQuery(mediaQueries.DESKTOP)
    const headerAccordionId: string = 'header-accordion'
    const userAccordionId: string = 'user-accordion'
    const headerImageSrc: string = useAssetPath('/images/logo.svg')
    const userProfileImage: ImageType = user?.image
    const logOutRoute: string = routes.LOG_OUT
    const shouldRenderUserLink: boolean = Boolean(user?.id)
    const shouldRenderAdminLink: boolean = actions?.includes(
        actionTypes.SITE_ADMIN_VIEW
    )
    /**
     * Handle expanding or collapsing accordions
     */
    const handleAccordionToggle = (id: string, isOpen: boolean) => {
        id === headerAccordionId &&
            domHelpers.lockBodyScroll(!isDesktop && isOpen)
        id === userAccordionId && setIsUserAccordionOpen(isOpen)
    }

    /**
     * Handle dismissing the confirmation modal on cancel
     */
    const handleLogoutCancel = () => setIsLogoutModalOpen(false)

    /**
     * Handle dismissing the confirmation modal on confirm and redirecting to logout route
     */
    const handleLogoutConfirm = () => {
        setIsLogoutModalOpen(false)
        window.location.href = logOutRoute
    }

    /**
     * Handle rendering the confirmation modal on logout
     */
    const handleLogoutRequest = (event: any): void => {
        event.preventDefault()
        setIsLogoutModalOpen(true)
    }

    /**
     * When the media breakpoint changes to desktop
     * ensure that the main nav is displayed and the body scroll lock is removed
     */
    useEffect(() => {
        domHelpers.lockBodyScroll(false)
        setIsMainAccordionOpen(isDesktop)
    }, [isDesktop])

    /**
     * Render
     */
    return (
        <header id="nav" className="c-site-header">
            <SkipLinks linkList={skipLinkList} />
            <LayoutWidthContainer className="c-site-header_content">
                <ErrorBoundary boundaryId="site-header">
                    <Link href="/">
                        <a className="c-site-header_logo u-focus-item">
                            <Image
                                src={headerImageSrc}
                                height={41}
                                width={231}
                                alt="FutureNHS home page"
                            />
                        </a>
                    </Link>
                    {(shouldRenderSearch || shouldRenderNavigation) && user && (
                        <div className="c-site-header_nav c-site-header-nav">
                            <Accordion
                                isOpen={isMainAccordionOpen}
                                id={headerAccordionId}
                                shouldCloseOnLeave={!isDesktop}
                                shouldCloseOnRouteChange={!isDesktop}
                                toggleOpenChildren="Menu"
                                toggleClosedChildren="Menu"
                                className="c-site-header-nav_content"
                                toggleClassName="c-site-header-nav_mobile-trigger c-site-header-nav_mobile-trigger--right"
                                contentClassName="c-site-header-nav_desktop-container"
                                toggleAction={handleAccordionToggle}
                            >
                                <>
                                    {shouldRenderSearch && (
                                        <Search
                                            method={requestMethods.GET}
                                            action="/search/"
                                            id="term"
                                            value={searchTerm}
                                            text={{
                                                label: 'Search the NHS website',
                                                placeholder: 'Search for...',
                                            }}
                                        />
                                    )}
                                    {shouldRenderNavigation && (
                                        <nav
                                            id="header-nav"
                                            className="c-site-header-nav_nav"
                                            aria-label="Navigation"
                                        >
                                            <ul
                                                className="c-site-header-nav_root-nav c-site-header-nav_root-nav--mobile-full-height"
                                                role="menubar"
                                            >
                                                <li role="none">
                                                    <Accordion
                                                        isOpen={
                                                            isUserAccordionOpen
                                                        }
                                                        id={userAccordionId}
                                                        className="c-site-header-nav_root-nav-item"
                                                        toggleClassName="c-site-header-nav_root-nav-trigger"
                                                        contentClassName="c-site-header-nav_sub-nav"
                                                        toggleOpenChildren={
                                                            <>
                                                                <span className="c-site-header-nav_root-nav-image">
                                                                    <Avatar
                                                                        image={
                                                                            userProfileImage
                                                                        }
                                                                        initials={
                                                                            userInitials
                                                                        }
                                                                    />
                                                                </span>
                                                                <span className="o-truncated-text-lines-1">
                                                                    {capitalise(
                                                                        {
                                                                            value: userName,
                                                                        }
                                                                    )}
                                                                </span>
                                                                <SVGIcon
                                                                    name={
                                                                        iconNames.CROSS_CIRCLE
                                                                    }
                                                                    className="c-site-header-nav_root-nav-icon"
                                                                />
                                                            </>
                                                        }
                                                        toggleClosedChildren={
                                                            <>
                                                                <span className="c-site-header-nav_root-nav-image">
                                                                    <Avatar
                                                                        image={
                                                                            userProfileImage
                                                                        }
                                                                        initials={
                                                                            userInitials
                                                                        }
                                                                    />
                                                                </span>
                                                                <span className="o-truncated-text-lines-1">
                                                                    {capitalise(
                                                                        {
                                                                            value: userName,
                                                                        }
                                                                    )}
                                                                </span>
                                                                <SVGIcon
                                                                    name={
                                                                        iconNames.PLUS_CIRCLE
                                                                    }
                                                                    className="c-site-header-nav_root-nav-icon"
                                                                />
                                                            </>
                                                        }
                                                        toggleAction={
                                                            handleAccordionToggle
                                                        }
                                                        shouldCloseOnLeave={
                                                            true
                                                        }
                                                    >
                                                        <div className="c-site-header-nav_sub-nav-content">
                                                            <ul
                                                                className={`u-list-none u-p-0 u-m-0`}
                                                                role="menu"
                                                                aria-label="Account navigation"
                                                            >
                                                                {shouldRenderUserLink && (
                                                                    <li
                                                                        className="c-site-header-nav_sub-nav-item"
                                                                        role="none"
                                                                    >
                                                                        <Link
                                                                            href={`${routes.USERS}/${user.id}`}
                                                                        >
                                                                            <a
                                                                                role="menuitem"
                                                                                className="c-site-header-nav_sub-nav-child"
                                                                            >
                                                                                {
                                                                                    editProfile
                                                                                }
                                                                            </a>
                                                                        </Link>
                                                                    </li>
                                                                )}
                                                                {shouldRenderAdminLink && (
                                                                    <li
                                                                        className="c-site-header-nav_sub-nav-item"
                                                                        role="none"
                                                                    >
                                                                        <Link
                                                                            href={
                                                                                routes.ADMIN
                                                                            }
                                                                        >
                                                                            <a
                                                                                role="menuitem"
                                                                                className="c-site-header-nav_sub-nav-child"
                                                                            >
                                                                                {
                                                                                    admin
                                                                                }
                                                                            </a>
                                                                        </Link>
                                                                    </li>
                                                                )}
                                                                <li
                                                                    className="c-site-header-nav_sub-nav-item"
                                                                    role="none"
                                                                >
                                                                    <Link
                                                                        href={
                                                                            logOutRoute
                                                                        }
                                                                    >
                                                                        <a
                                                                            className="c-button c-button--outline c-site-header-nav_sub-nav-child"
                                                                            onClick={
                                                                                handleLogoutRequest
                                                                            }
                                                                        >
                                                                            {
                                                                                logOut
                                                                            }
                                                                        </a>
                                                                    </Link>
                                                                    <Dialog
                                                                        id="dialog-logout"
                                                                        isOpen={
                                                                            isLogoutModalOpen
                                                                        }
                                                                        text={{
                                                                            cancelButton:
                                                                                logOutCancel,
                                                                            confirmButton:
                                                                                logOutConfirm,
                                                                            heading:
                                                                                logOutHeading,
                                                                        }}
                                                                        cancelAction={
                                                                            handleLogoutCancel
                                                                        }
                                                                        confirmAction={
                                                                            handleLogoutConfirm
                                                                        }
                                                                    >
                                                                        <p className="u-text-bold">
                                                                            {
                                                                                logOutBody
                                                                            }
                                                                        </p>
                                                                    </Dialog>
                                                                </li>
                                                            </ul>
                                                        </div>
                                                    </Accordion>
                                                </li>
                                                {navMenuList?.map(
                                                    (
                                                        {
                                                            url,
                                                            text,
                                                            isActive,
                                                            meta,
                                                        },
                                                        index
                                                    ) => {
                                                        const {
                                                            themeId,
                                                            iconName,
                                                        } = meta ?? {}

                                                        const generatedClasses =
                                                            {
                                                                link: classNames(
                                                                    'c-site-header-nav_root-nav-trigger u-border-theme-8',
                                                                    {
                                                                        [`c-site-header-nav_root-nav-trigger--active`]:
                                                                            isActive,
                                                                        [`u-border-theme-${themeId}`]:
                                                                            typeof themeId !==
                                                                            'undefined',
                                                                    }
                                                                ),
                                                                icon: classNames(
                                                                    'c-site-header-nav_root-nav-content-icon',
                                                                    {
                                                                        [`u-fill-theme-${themeId}`]:
                                                                            typeof themeId !==
                                                                            'undefined',
                                                                    }
                                                                ),
                                                            }

                                                        return (
                                                            <li
                                                                key={index}
                                                                role="none"
                                                                className="desktop:u-hidden"
                                                            >
                                                                <Link
                                                                    href={url}
                                                                >
                                                                    <a
                                                                        role="menuitem"
                                                                        aria-current={
                                                                            isActive
                                                                        }
                                                                        className={
                                                                            generatedClasses.link
                                                                        }
                                                                    >
                                                                        {iconName && (
                                                                            <SVGIcon
                                                                                name={
                                                                                    iconName
                                                                                }
                                                                                className={
                                                                                    generatedClasses.icon
                                                                                }
                                                                            />
                                                                        )}
                                                                        {text}
                                                                    </a>
                                                                </Link>
                                                            </li>
                                                        )
                                                    }
                                                )}
                                                <li
                                                    role="none"
                                                    className="u-margin-top-spacing-4 desktop:u-hidden"
                                                >
                                                    <span className="u-text-bold">
                                                        Need help?{' '}
                                                    </span>
                                                    <a
                                                        target="_blank"
                                                        rel="noopener"
                                                        href="http://support.future.nhs.uk"
                                                    >
                                                        Visit our support site
                                                    </a>
                                                </li>
                                            </ul>
                                        </nav>
                                    )}
                                </>
                            </Accordion>
                        </div>
                    )}
                </ErrorBoundary>
            </LayoutWidthContainer>
        </header>
    )
}
