import classNames from 'classnames';
import { useRouter } from 'next/router';

import { mediaQueries } from '@constants/css';
import Head from '@components/DocumentHead';
import { ErrorBoundary } from '@components/ErrorBoundary';
import { Header } from '@components/Header';
import { PhaseBanner } from '@components/PhaseBanner';
import { BreadCrumb } from '@components/BreadCrumb';
import { Footer } from '@components/Footer';
import { MainNav } from '@components/MainNav';
import { LayoutWidthContainer } from '@components/LayoutWidthContainer';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { RichText } from '@components/RichText';
import { CookieBanner } from '@components/CookieBanner';
import { mainNavMenuList, footerNavMenuList } from '@constants/navigation';
import { getBreadCrumbList } from '@helpers/routing/getBreadCrumb';
import { BreadCrumbList } from '@appTypes/routing';
import { useMediaQuery } from '@hooks/useMediaQuery';

import { Props } from './interfaces';

export const StandardLayout: (props: Props) => JSX.Element = ({
    routeId,
    shouldRenderSearch = true,
    shouldRenderUserNavigation = true,
    shouldRenderPhaseBanner = true,
    shouldRenderBreadCrumb = true,
    shouldRenderMainNav = true,
    user,
    actions,
    breadCrumbList,
    searchTerm,
    className,
    children
}) => {

    const router = useRouter();
    const isMobile: boolean = useMediaQuery(mediaQueries.MOBILE);

    const currentPathName: string = router?.pathname;
    const assetPath: string = process.env.NEXT_PUBLIC_ASSET_PREFIX || '';
    const breadCrumbDescriptionHtml: string = "<span class=\"u-text-bold\">Need help?</span> <a target=\"_blank\" rel=\"noopener\" href=\"https://futurenhstest.service-now.com/csm/?id=futurenhs_test\">Visit our support site</a>"

    footerNavMenuList.forEach((menuItem) => menuItem.isActive = menuItem.url === currentPathName);
    mainNavMenuList.forEach((menuItem) => { 
        
        menuItem.isActiveRoot = menuItem.url === '/' ? currentPathName === '/' :  currentPathName?.startsWith(menuItem.url); 
        menuItem.isActive = menuItem.url === currentPathName 
    
    });

    const currentRoutePathElements: Array<string> = router?.asPath?.split('/').filter((item) => item);
    const breadCrumbListToUse: BreadCrumbList = breadCrumbList ?? getBreadCrumbList({ pathElementList: currentRoutePathElements });
    const skipLinkList: Array<any> = [];

    if (shouldRenderMainNav && user) {

        skipLinkList.push({
            id: '#main-nav',
            text: 'Skip to main navigation',
            className: 'u-hidden tablet:u-block'
        });

        skipLinkList.push({
            id:'#header-accordion',
            text: 'Skip to main navigation',
            className: 'u-block tablet:u-hidden'
        });

    }

    skipLinkList.push({
        id: '#main',
        text: 'Skip to main content'
    });

    const generatedClasses: any = {
        wrapper: classNames('u-flex-grow', {
            ['u-bg-theme-3']: user,
            ['u-bg-theme-1']: !user
        }, className),
        breadCrumb: classNames('u-bg-theme-1', {
            ['u-hidden']: breadCrumbListToUse.length === 0
        }, 'tablet:u-block'),
        main: classNames('u-flex u-flex-grow'),
        content: classNames({
            ['u-m-0']: shouldRenderMainNav && user && isMobile,
            ['u-max-w-full']: shouldRenderMainNav && user && isMobile,
            ['u-w-0']: shouldRenderMainNav && user,
            ['u-w-full']: !shouldRenderMainNav && user
        })
    };

    return (

        <>
            <Head assetPath={assetPath} />
            <CookieBanner />
            <Header
                skipLinkList={skipLinkList}
                text={{
                    admin: 'Admin',
                    editProfile: 'My profile',
                    logOut: 'Log Off',
                    logOutHeading: 'Log Off',
                    logOutBody: 'Are you sure you would like to log off?',
                    logOutCancel: 'Cancel',
                    logOutConfirm: 'Yes, log off'
                }}
                user={user}
                actions={actions}
                shouldRenderSearch={shouldRenderSearch}
                shouldRenderNavigation={shouldRenderUserNavigation}
                navMenuList={mainNavMenuList}
                searchTerm={searchTerm} />
            <main className={generatedClasses.wrapper}>
                {shouldRenderPhaseBanner &&
                    <div className="u-bg-theme-3">
                        <LayoutWidthContainer>
                            <ErrorBoundary boundaryId="phase-banner">
                                <PhaseBanner text={{
                                    tag: 'beta',
                                    body: 'This is a new service – your <a href="http://www.google.co.uk">feedback</a> will help us to improve it.'
                                }} />
                            </ErrorBoundary>
                        </LayoutWidthContainer>
                    </div>
                }
                {shouldRenderBreadCrumb &&
                    <div className={generatedClasses.breadCrumb}>
                        <LayoutWidthContainer>
                            <ErrorBoundary boundaryId="bread-crumb">
                                <LayoutColumnContainer className="u-py-4">
                                    <LayoutColumn tablet={8}>
                                        <BreadCrumb
                                            text={{
                                                ariaLabel: 'Site breadcrumb'
                                            }}
                                            breadCrumbList={breadCrumbListToUse}
                                            truncationMinPathLength={8}
                                            truncationStartIndex={2}
                                            truncationEndIndex={6}
                                            className="u--mt-0.5 u-fill-theme-5" />
                                    </LayoutColumn>
                                    <LayoutColumn tablet={4} className="u-text-right u-hidden tablet:u-block">
                                        <RichText
                                            wrapperElementType="p"
                                            bodyHtml={breadCrumbDescriptionHtml}
                                            className="u-mb-0" />
                                    </LayoutColumn>
                                </LayoutColumnContainer>
                            </ErrorBoundary>
                        </LayoutWidthContainer>
                    </div>
                }
                <div className="u-overflow-hidden u-flex u-h-full">
                    <LayoutWidthContainer className={generatedClasses.main}>
                        <ErrorBoundary boundaryId="main-content">
                            {(shouldRenderMainNav && user) &&
                                <>
                                    <LayoutColumn hasGutters={false} mobile={0}>
                                        <MainNav navMenuList={mainNavMenuList} />
                                    </LayoutColumn>
                                    <LayoutColumn id="main" className={generatedClasses.content}>
                                        {children}
                                    </LayoutColumn>
                                </>
                            }
                            {(!shouldRenderMainNav || !user) &&
                                <div id="main" className={generatedClasses.content}>
                                    {children}
                                </div>
                            }
                        </ErrorBoundary>
                    </LayoutWidthContainer>
                </div>
            </main>
            <Footer
                text={{
                    title: 'Footer Navigation',
                    copyright: 'Crown copyright',
                    navMenuAriaLabel: 'Footer legal links'
                }}
                navMenuList={footerNavMenuList} />
        </>

    )

}