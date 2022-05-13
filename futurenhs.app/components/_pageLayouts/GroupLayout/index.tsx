import classNames from 'classnames'
import Head from 'next/head'

import { StandardLayout } from '@components/_pageLayouts/StandardLayout'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { GroupPageHeader } from '@components/GroupPageHeader'
import { ErrorBoundary } from '@components/ErrorBoundary'
import { getGroupNavMenuList } from '@helpers/routing/getGroupNavMenuList'
import { getBreadCrumbList } from '@helpers/routing/getBreadCrumb'
import { BreadCrumbList } from '@appTypes/routing'

import { Props } from './interfaces'

export const GroupLayout: (props: Props) => JSX.Element = ({
    tabId,
    themeId,
    entityText,
    image,
    actions,
    routes,
    children,
    shouldRenderGroupHeader = true,
    pageTitle,
    ...rest
}) => {

    /**
     * TODO: Determine whether user has access to group when back-end permissions work is complete
     */
    const hasAccessToGroup: boolean = true

    const navMenuList = getGroupNavMenuList({
        groupRoute: routes.groupRoot,
        activeId: tabId,
        isRestricted: !hasAccessToGroup
    })

    const currentRoutePathElements: Array<string> =
        routes.groupRoot?.split('/')?.filter((item) => item) ?? []
    const breadCrumbList: BreadCrumbList = getBreadCrumbList({
        pathElementList: currentRoutePathElements,
    })

    const { title, metaDescription, mainHeading, strapLine } = entityText ?? {}

    return (
        <StandardLayout
            breadCrumbList={breadCrumbList}
            actions={actions}
            className="u-bg-theme-3"
            {...rest}
        >
            <Head>
                <meta name="description" content={metaDescription} />
            </Head>
            <LayoutColumnContainer>
                {shouldRenderGroupHeader && (
                    <ErrorBoundary boundaryId="group-page-header">
                        <GroupPageHeader
                            id="group"
                            text={{
                                mainHeading: mainHeading,
                                description: strapLine,
                                navMenuTitle: 'Group menu',
                            }}
                            image={image}
                            themeId={themeId}
                            actions={actions}
                            routes={routes}
                            navMenuList={navMenuList}
                        />
                    </ErrorBoundary>
                )}
                <ErrorBoundary boundaryId="group-page-body">
                    {children}
                </ErrorBoundary>
            </LayoutColumnContainer>
        </StandardLayout>
    )
}
