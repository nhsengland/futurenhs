import { useState } from 'react';
import classNames from 'classnames';
import Link from 'next/link';
import Image from 'next/image';

import { LayoutWidthContainer } from '@components/LayoutWidthContainer';
import { Search } from '@components/Search';
import { SVGIcon } from '@components/SVGIcon';
import { SkipLinks } from '@components/SkipLinks';
import { Accordion } from '@components/Accordion';
import { Dialog } from '@components/Dialog';
import { Avatar } from '@components/Avatar';
import { useMediaQuery } from '@hooks/useMediaQuery';
import * as domHelpers from '@helpers/dom';
import { cssUtilityClasses, mediaQueries } from '@constants/css';
import { iconNames } from '@constants/icons';
import { routes } from '@constants/routes';

import { Props } from './interfaces'

export const Header: (props: Props) => JSX.Element = ({
    skipLinkList = [
        {
            id: '#main-nav',
            text: 'Skip to main navigation'
        },
        {
            id: '#main',
            text: 'Skip to main content'
        }
    ],
    shouldRenderSearch = true,
    shouldRenderNavigation = true,
    navMenuList,
    content,
    user
}) => {

    const [isLogoutModalOpen, setIsLogoutModalOpen] = useState(false);
    const [isUserAccordionOpen, setIsUserAccordionOpen] = useState(false);

    const { initialsText, fullNameText } = user ?? {};
    const { editProfileText,
            logOutText,
            logOutHeadingText, 
            logOutBodyText,
            logOutCancelText,
            logOutConfirmText } = content ?? {};

    const headerAccordionId: string = 'header-accordion';
    const userAccordionId: string = 'user-accordion';
    const logOutRoute: string = routes.LOG_OUT;
    const isMobile: boolean = useMediaQuery(mediaQueries.MOBILE);
    const getAccordionIcon = (isOpen: boolean) => isOpen ? iconNames.CROSS_CIRCLE : iconNames.PLUS_CIRCLE;
    const handleAccordionToggle = (id: string, isOpen: boolean) => {
        
        id === headerAccordionId && domHelpers.lockBodyScroll(isMobile && isOpen);
        id === userAccordionId && setIsUserAccordionOpen(isOpen);

    };
    const handleLogoutCancel = () => setIsLogoutModalOpen(false);
    const handleLogoutConfirm = () => {
        setIsLogoutModalOpen(false);
        window.location.href = logOutRoute;
    };
    const handleLogoutRequest = (event: any): void => { 
        event.preventDefault();
        setIsLogoutModalOpen(true);
    };

    return (

        <header id="nav" className="c-site-header">
            <SkipLinks linkList={skipLinkList} />
            <LayoutWidthContainer className="c-site-header_content">
                <Link href="/">
                    <a className="c-site-header_logo u-focus-item">
                        <Image 
                            src="/images/logo.svg" 
                            height={41} 
                            width={231} 
                            alt="Future NHS home page" />
                    </a>
                </Link>
                {(shouldRenderSearch || shouldRenderNavigation) && 
                    <div className="c-site-header_nav c-site-header-nav">
                        <Accordion
                            isOpen={!isMobile}
                            id={headerAccordionId}
                            toggleChildren={"Menu"}
                            className="c-site-header-nav_content"
                            toggleClassName="c-site-header-nav_mobile-trigger c-site-header-nav_mobile-trigger--right"
                            contentClassName="c-site-header-nav_desktop-container"
                            toggleAction={handleAccordionToggle}>
                                <>
                                    {shouldRenderSearch &&
                                        <Search 
                                            method="GET" 
                                            action="/search/" 
                                            id="term"
                                            content={{
                                                labelText: "Search the NHS website",
                                                placeholderText: "Search for..."
                                            }} />
                                    }
                                    {shouldRenderNavigation &&
                                        <nav id="header-nav" className="c-site-header-nav_nav" aria-label="Navigation">
                                            <ul className="c-site-header-nav_root-nav c-site-header-nav_root-nav--mobile-full-height" role="menubar">
                                                <li role="none">
                                                    <Accordion
                                                        isOpen={isUserAccordionOpen}
                                                        id={userAccordionId}
                                                        className="c-site-header-nav_root-nav-item"
                                                        toggleClassName="c-site-header-nav_root-nav-trigger"
                                                        contentClassName="c-site-header-nav_sub-nav"
                                                        toggleChildren={
                                                            <>
                                                                <span className="c-site-header-nav_root-nav-image">
                                                                    <Avatar initials={initialsText} />
                                                                </span>
                                                                <span>{fullNameText}</span>
                                                                <SVGIcon name={getAccordionIcon(isUserAccordionOpen)} className="c-site-header-nav_root-nav-icon" />
                                                            </>
                                                        }
                                                        toggleAction={handleAccordionToggle}
                                                        shouldCloseOnLeave={true}>
                                                            <div className="c-site-header-nav_sub-nav-content">
                                                                <ul className={`u-list-none u-p-0 u-m-0`} role="menu" aria-label="Account navigation">
                                                                    <li className="c-site-header-nav_sub-nav-item" role="none">
                                                                        <Link href="/todo">
                                                                            <a role="menuitem" className="c-site-header-nav_sub-nav-child">{editProfileText}</a>
                                                                        </Link>
                                                                    </li>
                                                                    <li className="c-site-header-nav_sub-nav-item" role="none">
                                                                        <Link href={logOutRoute}>
                                                                            <a className="c-button c-button--outline c-site-header-nav_sub-nav-child" onClick={handleLogoutRequest}>
                                                                                {logOutText}
                                                                            </a>
                                                                        </Link>
                                                                        <Dialog 
                                                                            id="dialog-logout"
                                                                            isOpen={isLogoutModalOpen}
                                                                            content={{
                                                                                cancelButtonText: logOutCancelText,
                                                                                confirmButtonText: logOutConfirmText
                                                                            }}
                                                                            cancelAction={handleLogoutCancel}
                                                                            confirmAction={handleLogoutConfirm}>
                                                                                <h3>{logOutHeadingText}</h3>
                                                                                <p className={cssUtilityClasses.TEXT_BOLD}>{logOutBodyText}</p>
                                                                        </Dialog>
                                                                    </li>
                                                                </ul>
                                                            </div>
                                                    </Accordion>
                                                </li>
                                                {navMenuList?.map(({ url, text, isActive, meta }, index) => {

                                                    const { themeId, iconName } = meta ?? {};

                                                    const generatedClasses = {
                                                        link: classNames('c-site-header-nav_root-nav-trigger u-border-theme-8', {
                                                            [`c-site-header-nav_root-nav-trigger--active`]: isActive,
                                                            [`u-border-theme-8-${themeId}`]: typeof themeId !== 'undefined'
                                                        }),
                                                        icon: classNames('c-site-header-nav_root-nav-content-icon', {
                                                            [`u-fill-theme-${themeId}`]: typeof themeId !== 'undefined'
                                                        })
                                                    }

                                                    return (

                                                        <li key={index} role="none" className={cssUtilityClasses.DESKTOP_HIDDEN}>
                                                            <Link href={url}>
                                                                <a role="menuitem" aria-current={isActive} className={generatedClasses.link}>
                                                                    {iconName &&
                                                                        <SVGIcon name={iconName} className={generatedClasses.icon} />
                                                                    }
                                                                    {text}
                                                                </a>
                                                            </Link>
                                                        </li>

                                                    )

                                                })}
                                                <li role="none" className={`u-margin-top-spacing-4 ${cssUtilityClasses.DESKTOP_HIDDEN}`}>
                                                    <span className={cssUtilityClasses.TEXT_BOLD}>Need help? </span>
                                                    <a target="_blank" rel="noopener" href="https://futurenhstest.service-now.com/csm/?id=futurenhs_test">Visit our support site</a>
                                                </li>
                                            </ul>
                                        </nav>
                                    }
                                </>
                        </Accordion>
                    </div>
                }
            </LayoutWidthContainer>
        </header>

    )

}
