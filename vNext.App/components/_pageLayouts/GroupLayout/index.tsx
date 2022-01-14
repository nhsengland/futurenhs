import classNames from 'classnames';
import Head from 'next/head';
import { useRouter } from 'next/router';

import { StandardLayout } from '@components/_pageLayouts/StandardLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { PageHeader } from '@components/PageHeader';
import { getGroupNavMenuList } from '@helpers/routing/getGroupNavMenuList';
import { getRouteToParam } from '@helpers/routing/getRouteToParam';
import { getBreadCrumbList } from '@helpers/routing/getBreadCrumb';
import { BreadCrumbList } from '@appTypes/routing';

import { Props } from './interfaces';

export const GroupLayout: (props: Props) => JSX.Element = ({
    id,
    content,
    image,
    children,
    ...rest 
}) => {

    const router = useRouter();

    const navMenuList = getGroupNavMenuList({
        router: router,
        activeId: id
    });

    const groupRoute: string = getRouteToParam({ 
        router: router,
        paramName: 'group' 
    });
    
    const currentRoutePathElements: Array<string> = groupRoute?.split('/').filter((item) => item);
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
                    navMenuList={navMenuList}
                    className="u-bg-theme-14" />
                {children}
            </LayoutColumnContainer>
        </StandardLayout>

    )
    
}