import Image from 'next/image';
import { useState, useEffect, useCallback } from 'react';
import classNames from 'classnames';

import { LayoutColumn } from '@components/LayoutColumn';
import { RichText } from '@components/RichText';
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
    content,
    navMenuList,
    className
}) => {

    const [isMenuAccordionOpen, setIsMenuAccordionOpen] = useState(true);

    const { mainHeadingHtml, descriptionHtml, navMenuTitleText } = content ?? {};
    const hasMenuItems: boolean = navMenuList?.length > 0;
    const isMobile: boolean = useMediaQuery(mediaQueries.MOBILE);

    const generatedClasses: any = {
        wrapper: classNames('c-page-header', className),
        header: classNames('c-page-header_header'),
        heading: classNames('u-mb-1', 'u-text-theme-1'),
        hero: classNames('c-page-header_hero'),
        heroBody: classNames('c-page-header_hero-body'),
        description: classNames('c-page-header_description'),
        navTrigger: classNames('c-page-header_nav-trigger'),
        navTriggerIcon: classNames('c-page-header_nav-trigger-icon'),
        navContent: classNames('c-page-header_nav-content')
    };

    const getAccordionIcon = useCallback((isOpen: boolean) => isOpen ? iconNames.CHEVRON_UP : iconNames.CHEVRON_DOWN, [isMenuAccordionOpen]);
    const handleAccordionToggle = useCallback((_, isOpen: boolean) => setIsMenuAccordionOpen(isOpen), [isMenuAccordionOpen]);

    useEffect(() => {

        setIsMenuAccordionOpen(!isMobile);

    }, [isMobile]);

    return (

        <div className={generatedClasses.wrapper}>
            <LayoutColumn>
                <div className={generatedClasses.header}>
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
                        {mainHeadingHtml}
                    </h1>
                    {descriptionHtml &&
                        <RichText 
                            className={generatedClasses.description} 
                            wrapperElementType="p"
                            bodyHtml={descriptionHtml} />
                    }
                </div>
                {hasMenuItems &&
                    <Accordion  
                        id={id}
                        isOpen={isMenuAccordionOpen}
                        toggleAction={handleAccordionToggle}
                        toggleClassName={generatedClasses.navTrigger}
                        toggleChildren={
                            <>
                                {navMenuTitleText}
                                <SVGIcon name={getAccordionIcon(isMenuAccordionOpen)} className={generatedClasses.navTriggerIcon} />
                            </>
                        }
                        className={generatedClasses.navContent}>
                            <TabbedNav 
                                content={{
                                    ariaLabelText: navMenuTitleText
                                }}
                                navMenuList={navMenuList} />
                    </Accordion>
                }
            </LayoutColumn>
        </div>

    )

}
