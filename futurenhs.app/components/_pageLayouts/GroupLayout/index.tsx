import classNames from 'classnames';
import Head from 'next/head';
import { useRouter } from 'next/router';

import { routeParams } from '@constants/routes';
import { StandardLayout } from '@components/_pageLayouts/StandardLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { GroupPageHeader } from '@components/GroupPageHeader';
import { ErrorBoundary } from '@components/ErrorBoundary';
import { getActionNavMenuList } from '@helpers/actions/getActionNavMenuList';
import { getGroupNavMenuList } from '@helpers/routing/getGroupNavMenuList';
import { getRouteToParam } from '@helpers/routing/getRouteToParam';
import { getBreadCrumbList } from '@helpers/routing/getBreadCrumb';
import { BreadCrumbList } from '@appTypes/routing';

import { Props } from './interfaces';

export const GroupLayout: (props: Props) => JSX.Element = ({
    id,
    text,
    image,
    theme,
    actions,
    children,
    ...rest 
}) => {

    const router = useRouter();

    const groupRoute: string = getRouteToParam({ 
        router: router,
        paramName: routeParams.GROUPID,
        shouldIncludeParam: true
    });

    const actionsMenuList = getActionNavMenuList({
        groupRoute: groupRoute,
        actions: actions
    });

    const navMenuList = getGroupNavMenuList({
        groupRoute: groupRoute,
        activeId: id
    });
    
    const currentRoutePathElements: Array<string> = groupRoute?.split('/')?.filter((item) => item) ?? [];
    const breadCrumbList: BreadCrumbList = getBreadCrumbList({ pathElementList: currentRoutePathElements });

    const { title, 
            metaDescription, 
            mainHeading,
            strapLine } = text ?? {};

    return (

        <StandardLayout breadCrumbList={breadCrumbList} {...rest}>
            <Head>
                <title>{title}</title>
                <meta name="description" content={metaDescription} />
            </Head>
            <LayoutColumnContainer>
                <ErrorBoundary boundaryId="group-page-header">
                    <GroupPageHeader 
                        id="group"
                        text={{
                            mainHeading: mainHeading, 
                            description: strapLine,
                            navMenuTitle: 'Group menu'
                        }}
                        image={image}
                        theme={theme}
                        shouldRenderActionsMenu={true}
                        actionsMenuList={actionsMenuList}
                        navMenuList={navMenuList} />
                </ErrorBoundary>
                <ErrorBoundary boundaryId="group-page-body">
                    {children}
                </ErrorBoundary>
            </LayoutColumnContainer>
        </StandardLayout>

    )
    
}