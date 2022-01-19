import classNames from 'classnames';
import Head from 'next/head';
import { useRouter } from 'next/router';

import { routeParams } from '@constants/routes';
import { StandardLayout } from '@components/_pageLayouts/StandardLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { PageHeader } from '@components/PageHeader';
import { getActionNavMenuList } from '@helpers/actions/getActionNavMenuList';
import { getGroupNavMenuList } from '@helpers/routing/getGroupNavMenuList';
import { getRouteToParam } from '@helpers/routing/getRouteToParam';
import { getBreadCrumbList } from '@helpers/routing/getBreadCrumb';
import { BreadCrumbList } from '@appTypes/routing';

import { Props } from './interfaces';

export const GroupLayout: (props: Props) => JSX.Element = ({
    id,
    content,
    image,
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
        actions: actions
    });

    const navMenuList = getGroupNavMenuList({
        groupRoute: groupRoute,
        activeId: id
    });
    
    const currentRoutePathElements: Array<string> = groupRoute?.split('/')?.filter((item) => item) ?? [];
    const breadCrumbList: BreadCrumbList = getBreadCrumbList({ pathElementList: currentRoutePathElements });

    const { titleText, 
            metaDescriptionText, 
            mainHeadingHtml,
            strapLineText } = content ?? {};

    return (

        <StandardLayout breadCrumbList={breadCrumbList} {...rest}>
            <Head>
                <title>{titleText}</title>
                <meta name="description" content={metaDescriptionText} />
            </Head>
            <LayoutColumnContainer>
                <PageHeader 
                    id="group"
                    content={{
                        mainHeadingHtml: mainHeadingHtml, 
                        descriptionHtml: strapLineText,
                        navMenuTitleText: 'Group menu'
                    }}
                    image={image}
                    shouldRenderActionsMenu={true}
                    actionsMenuList={actionsMenuList}
                    navMenuList={navMenuList}
                    className="u-bg-theme-14" />
                {children}
            </LayoutColumnContainer>
        </StandardLayout>

    )
    
}