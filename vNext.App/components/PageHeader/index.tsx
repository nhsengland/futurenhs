import Link from 'next/link';
import { useState, useEffect, useCallback } from 'react';
import classNames from 'classnames';

import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { RichText } from '@components/RichText';
import { Image } from '@components/Image';
import { SVGIcon } from '@components/SVGIcon';
import { Accordion } from '@components/Accordion';
import { TabbedNav } from '@components/TabbedNav';
import { mediaQueries } from '@constants/css';
import { iconNames } from '@constants/icons';
import { useMediaQuery } from '@hooks/useMediaQuery';

import { Props } from './interfaces';

export const PageHeader: (props: Props) => JSX.Element = ({
    id,
    image,
    text,
    shouldRenderActionsMenu,
    actionsMenuList,
    navMenuList,
    className
}) => {

    const [isActionsAccordionOpen, setIsActionsAccordionOpen] = useState(false);
    const [isMenuAccordionOpen, setIsMenuAccordionOpen] = useState(true);

    const actionsMenuTitleText: string = 'Actions';
    const { mainHeading, 
            description, 
            navMenuTitle } = text ?? {};

    const hasActionsMenuItems: boolean = actionsMenuList?.length > 0;
    const hasMenuItems: boolean = navMenuList?.length > 0;
    const isMobile: boolean = useMediaQuery(mediaQueries.MOBILE);

    const generatedIds: any = {
        actionsAccordion: `${id}-actions`,
        menuAccordion: `${id}-menu`
    };

    const generatedClasses: any = {
        wrapper: classNames('c-page-header', className),
        header: classNames('c-page-header_header'),
        heading: classNames('u-mb-2', 'u-text-theme-1', 'o-truncated-text-lines-3'),
        hero: classNames('c-page-header_hero'),
        heroBody: classNames('c-page-header_hero-body'),
        description: classNames('c-page-header_description', 'o-truncated-text-lines-2', 'u-m-0'),
        actions: classNames('c-page-header_actions', 'u-relative'),
        actionsTrigger: classNames('c-page-header_actions-trigger'),
        actionsTriggerIcon: classNames('c-page-header_actions-trigger-icon'),
        actionsContent: classNames('c-page-header_actions-content', 'u-list-none', 'u-pt-1.5'),
        navTrigger: classNames('c-page-header_nav-trigger'),
        navTriggerIcon: classNames('c-page-header_nav-trigger-icon'),
        navContent: classNames('c-page-header_nav-content')
    };

    const getAccordionIcon = useCallback((isOpen: boolean) => isOpen ? iconNames.CHEVRON_UP : iconNames.CHEVRON_DOWN, [isActionsAccordionOpen, isMenuAccordionOpen]);
    const handleAccordionToggle = useCallback((id, isOpen: boolean) => {
        
        id === generatedIds.actionsAccordion && setIsActionsAccordionOpen(isOpen);
        id === generatedIds.menuAccordion && setIsMenuAccordionOpen(isOpen);
        
    }, [isActionsAccordionOpen, isMenuAccordionOpen]);

    useEffect(() => {

        setIsMenuAccordionOpen(!isMobile);

    }, [isMobile]);

    return (

        <div className={generatedClasses.wrapper}>
            <LayoutColumn>
                <LayoutColumnContainer className={generatedClasses.header}>
                    <LayoutColumn tablet={8} desktop={9}>
                        {image &&
                            <div className={generatedClasses.hero}>
                                <div className={generatedClasses.heroBody}>
                                    <Image 
                                        src={image.src} 
                                        alt={image.altText}
                                        height={image.height}
                                        width={image.width} />
                                </div>
                            </div>
                        }
                        <h1 className={generatedClasses.heading}>
                            {mainHeading}
                        </h1>
                        {description &&
                            <RichText 
                                className={generatedClasses.description} 
                                wrapperElementType="p"
                                bodyHtml={description} />
                        }
                    </LayoutColumn>
                    {(shouldRenderActionsMenu && hasActionsMenuItems) &&
                        <LayoutColumn tablet={4} desktop={3} className="u-self-end">
                            <Accordion  
                                id={generatedIds.actionsAccordion}
                                isOpen={isActionsAccordionOpen}
                                shouldCloseOnLeave={true}
                                toggleAction={handleAccordionToggle}
                                toggleClassName={generatedClasses.actionsTrigger}
                                toggleChildren={
                                    <>
                                        {actionsMenuTitleText}
                                        <SVGIcon name={getAccordionIcon(isActionsAccordionOpen)} className={generatedClasses.actionsTriggerIcon} />
                                    </>
                                }
                                className={generatedClasses.actions}>
                                    <ul className={generatedClasses.actionsContent}>
                                        {actionsMenuList.map(({ url, text }, index) => {

                                            return (

                                                <li key={index} className="u-m-0">
                                                    <Link href={url}>
                                                        <a className="c-page-header_actions-content-item u-m-0 u-block u-break-words">
                                                            {text}
                                                        </a>
                                                    </Link>
                                                </li>

                                            )

                                        })}
                                    </ul>
                            </Accordion>
                        </LayoutColumn>
                    }
                </LayoutColumnContainer>
                {hasMenuItems &&
                    <Accordion  
                        id={generatedIds.menuAccordion}
                        isOpen={isMenuAccordionOpen}
                        toggleAction={handleAccordionToggle}
                        toggleClassName={generatedClasses.navTrigger}
                        toggleChildren={
                            <>
                                {navMenuTitle}
                                <SVGIcon name={getAccordionIcon(isMenuAccordionOpen)} className={generatedClasses.navTriggerIcon} />
                            </>
                        }
                        className={generatedClasses.navContent}>
                            <TabbedNav 
                                text={{
                                    ariaLabel: navMenuTitle
                                }}
                                navMenuList={navMenuList} />
                    </Accordion>
                }
            </LayoutColumn>
        </div>

    )

}
