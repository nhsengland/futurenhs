import { useState, useCallback } from 'react';
import classNames from 'classnames';
import Head from 'next/head';
import { useRouter } from 'next/router';

import { useMediaQuery } from '@hooks/useMediaQuery';
import { iconNames } from '@constants/icons';
import { mediaQueries } from '@constants/css';
import { PageBody } from '@components/PageBody';
import { SVGIcon } from '@components/SVGIcon';
import { Accordion } from '@components/Accordion';
import { TabbedNav } from '@components/TabbedNav';
import { StandardLayout } from '@components/_pageLayouts/StandardLayout';
import { getBreadCrumbList } from '@helpers/routing/getBreadCrumb';
import { BreadCrumbList } from '@appTypes/routing';

import { Props } from './interfaces';

export const AdminLayout: (props: Props) => JSX.Element = ({
    contentText,
    user,
    actions,
    children
}) => {

    const { pathname } = useRouter();

    const [isMenuAccordionOpen, setIsMenuAccordionOpen] = useState(true);
    const isMobile: boolean = useMediaQuery(mediaQueries.MOBILE);
    const navMenuList: Array<any> = [
        {
            url: '/admin/users',
            text: 'Manage users',
            isActive: pathname === '/admin/users'
        },
        {
            url: '/admin/groups',
            text: 'Manage groups',
            isActive: pathname === '/admin/groups'
        }
    ];
    const activeMenuItemText: string = navMenuList?.find(({ isActive }) => isActive)?.text;
    const currentRoutePathElements: Array<string> = '/admin'.split('/')?.filter((item) => item) ?? [];
    const breadCrumbList: BreadCrumbList = getBreadCrumbList({ pathElementList: currentRoutePathElements });

    const { metaDescription,
            title,
            mainHeading } = contentText ?? {};

    const generatedIds: any = {
        menuAccordion: `admin-menu`
    };

    const generatedClasses = {
        navTrigger: classNames('c-page-header_nav-trigger'),
        navTriggerIcon: classNames('c-page-header_nav-trigger-icon'),
        navContent: classNames('c-page-header_nav-content', 'u-text-theme-8', 'u-border-theme-0')
    }

    const getAccordionIcon = useCallback((isOpen: boolean) => isOpen ? iconNames.CHEVRON_UP : iconNames.CHEVRON_DOWN, [isMenuAccordionOpen]);
    const handleAccordionToggle = useCallback((id, isOpen: boolean) => {
        
        id === generatedIds.menuAccordion && setIsMenuAccordionOpen(isOpen);
        
    }, [isMenuAccordionOpen]);

    return (

        <StandardLayout
            user={user}
            actions={actions}
            breadCrumbList={breadCrumbList}
            className="u-bg-theme-3">
                <Head>
                    <title>{title}</title>
                    <meta name="description" content={metaDescription} />
                </Head>
                <div className="u-px-4 u-py-10">
                    <h1>{mainHeading}</h1>
                    <Accordion  
                        id={generatedIds.menuAccordion}
                        isOpen={isMenuAccordionOpen}
                        shouldCloseOnLeave={isMobile}
                        toggleAction={handleAccordionToggle}
                        toggleClassName={generatedClasses.navTrigger}
                        toggleChildren={
                            <>
                                {activeMenuItemText}
                                <SVGIcon name={getAccordionIcon(isMenuAccordionOpen)} className={generatedClasses.navTriggerIcon} />
                            </>
                        }
                        className={generatedClasses.navContent}>
                            <TabbedNav 
                                text={{
                                    ariaLabel: ''
                                }}
                                navMenuList={navMenuList} />
                    </Accordion>
                </div>
                <PageBody>
                    {children}
                </PageBody>
        </StandardLayout>

    )
    
}