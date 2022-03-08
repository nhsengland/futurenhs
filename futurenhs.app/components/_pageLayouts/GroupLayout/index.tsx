import classNames from 'classnames';
import Head from 'next/head';
import { useRouter } from 'next/router';

import { routeParams } from '@constants/routes';
import { StandardLayout } from '@components/_pageLayouts/StandardLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { GroupPageHeader } from '@components/GroupPageHeader';
import { ErrorBoundary } from '@components/ErrorBoundary';
import { getGroupNavMenuList } from '@helpers/routing/getGroupNavMenuList';
import { getRouteToParam } from '@helpers/routing/getRouteToParam';
import { getBreadCrumbList } from '@helpers/routing/getBreadCrumb';
import { BreadCrumbList } from '@appTypes/routing';

import { Props } from './interfaces';

export const GroupLayout: (props: Props) => JSX.Element = ({
    tabId,
    themeId,
    entityText,
    image,
    actions,
    children,
    shouldRenderGroupHeader = true,
    ...rest 
}) => {

    const router = useRouter();

    const groupRoute: string = getRouteToParam({ 
        router: router,
        paramName: routeParams.GROUPID,
        shouldIncludeParam: true
    });

    const navMenuList = getGroupNavMenuList({
        groupRoute: groupRoute,
        activeId: tabId
    });
    
    const currentRoutePathElements: Array<string> = groupRoute?.split('/')?.filter((item) => item) ?? [];
    const breadCrumbList: BreadCrumbList = getBreadCrumbList({ pathElementList: currentRoutePathElements });

    const { title, 
            metaDescription, 
            mainHeading,
            strapLine } = entityText ?? {};

    return (

        <StandardLayout 
            breadCrumbList={breadCrumbList} 
            actions={actions}
            className="u-bg-theme-3"
            {...rest}>
                <Head>
                    <title>{title}</title>
                    <meta name="description" content={metaDescription} />
                </Head>
                <LayoutColumnContainer>
                    {shouldRenderGroupHeader &&
                        <ErrorBoundary boundaryId="group-page-header">
                            <GroupPageHeader 
                                id="group"
                                text={{
                                    mainHeading: mainHeading, 
                                    description: strapLine,
                                    navMenuTitle: 'Group menu'
                                }}
                                image={image}
                                themeId={themeId}
                                actions={actions}
                                navMenuList={navMenuList} />
                        </ErrorBoundary>
                    }
                    <ErrorBoundary boundaryId="group-page-body">
                        {children}
                    </ErrorBoundary>
                </LayoutColumnContainer>
        </StandardLayout>

    )
    
}