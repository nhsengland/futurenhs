import classNames from 'classnames';
import { useRouter } from 'next/router';

import Head from '@components/DocumentHead';
import { Header } from '@components/Header';
import { PhaseBanner } from '@components/PhaseBanner';
import { BreadCrumb } from '@components/BreadCrumb';
import { Footer } from '@components/Footer';
import { MainNav } from '@components/MainNav';
import { LayoutWidthContainer } from '@components/LayoutWidthContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { mainNavMenuList, footerNavMenuList } from '@constants/navigation';

import { Props } from './interfaces';

export const Layout: (props: Props) => JSX.Element = ({ 
    shouldRenderSearch = true,
    shouldRenderUserNavigation = true,
    shouldRenderPhaseBanner = true,
    shouldRenderBreadCrumb = true, 
    shouldRenderMainNav = true,
    user,
    className, 
    children 
}) => {

    const router = useRouter();
    const currentPathName: string = router?.pathname;
    const assetPath: string = process.env.NEXT_PUBLIC_ASSET_PREFIX || '';
    const breadCrumbDescriptionHtml: string = "<span class=\"u-text-bold\">Need help?</span> <a target=\"_blank\" rel=\"noopener\" href=\"https://futurenhstest.service-now.com/csm/?id=futurenhs_test\">Visit our support site</a>" 

    mainNavMenuList.forEach((menuItem) => menuItem.isActive = menuItem.url === currentPathName);
    footerNavMenuList.forEach((menuItem) => menuItem.isActive = menuItem.url === currentPathName);

    const skipLinkList = [];

    if(shouldRenderMainNav){

        skipLinkList.push({
            id: '#main-nav',
            text: 'Skip to main navigation'
        });

    }

    skipLinkList.push({
        id: '#main',
        text: 'Skip to main content'
    });

    const generatedClasses: any = {
        wrapper: classNames('u-flex-grow', 'u-bg-theme-3', className),
        breadCrumb: classNames('u-bg-theme-1', 'u-tablet-up'),
        main: classNames('u-flex u-flex-grow'),
        content: classNames('u-w-0')
    };

    return (

        <>
            <Head assetPath={assetPath} />
            <Header
                skipLinkList={skipLinkList}
                content={{
                    editProfileText: 'Edit profile',
                    logOutText: 'Log out',
                    logOutHeadingText: 'Log out', 
                    logOutBodyText: 'Are you sure you would like to log out?',
                    logOutCancelText: 'Cancel',
                    logOutConfirmText: 'Yes, log out'
                }}
                user={user}
                shouldRenderSearch={shouldRenderSearch} 
                shouldRenderNavigation={shouldRenderUserNavigation}
                navMenuList={mainNavMenuList} />
            <main id="main" className={generatedClasses.wrapper}>
                {shouldRenderPhaseBanner &&
                    <div className="u-bg-theme-3">
                        <LayoutWidthContainer>
                            <PhaseBanner content={{
                                tagText: 'alpha',
                                bodyHtml: 'This is a new service – your <a href="http://www.google.co.uk">feedback</a> will help us to improve it.'
                            }} />
                        </LayoutWidthContainer>
                    </div>
                }
                {shouldRenderBreadCrumb &&
                    <div className={generatedClasses.breadCrumb}>
                        <LayoutWidthContainer>
                            <BreadCrumb
                                navMenuList={[]} 
                                content={{
                                    descriptionHtml: breadCrumbDescriptionHtml
                                }} />
                        </LayoutWidthContainer>
                    </div>
                }
                <div className="u-overflow-hidden u-flex u-h-full">
                    <LayoutWidthContainer className={generatedClasses.main}>
                        {shouldRenderMainNav &&
                            <LayoutColumn hasGutters={false} mobile={0}>
                                <MainNav navMenuList={mainNavMenuList}/>
                            </LayoutColumn>
                        }
                        <LayoutColumn className={generatedClasses.content}>
                            {children}
                        </LayoutColumn>
                    </LayoutWidthContainer>
                </div>
            </main>
            <Footer
                content={{
                    titleText: 'Footer Navigation',
                    copyrightText: 'Crown copyright',
                    navMenuAriaLabelText: 'Footer legal links'
                }}
                navMenuList={footerNavMenuList} />
        </>

    )
    
}