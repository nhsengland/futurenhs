import Head from 'next/head'
import { useRouter } from 'next/router'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { GroupPageHeader } from '@components/blocks/GroupPageHeader'
import { ErrorBoundary } from '@components/layouts/ErrorBoundary'
import { getGroupNavMenuList } from '@helpers/routing/getGroupNavMenuList'
import { getBreadCrumbList } from '@helpers/routing/getBreadCrumb'
import { BreadCrumbList } from '@appTypes/routing'
import { GroupsPageTextContent } from '@appTypes/content'
import { Image } from '@appTypes/image'
import { Routes } from '@appTypes/routing'
import { User } from '@appTypes/user'
import StandardLayout from '@components/layouts/pages/StandardLayout'
import { FeatureFlag } from '@services/getUserFeatureFlags'
import { GroupTabId } from './interfaces'

export interface Props {
    tabId: GroupTabId
    routes: Routes
    shouldRenderSearch?: boolean
    shouldRenderUserNavigation?: boolean
    shouldRenderPhaseBanner?: boolean
    shouldRenderBreadCrumb?: boolean
    shouldRenderMainNav?: boolean
    shouldRenderGroupHeader?: boolean
    user?: User
    actions?: any
    memberStatus?: string
    themeId?: string
    className?: string
    entityText?: GroupsPageTextContent
    image?: Image
    children?: any
    pageTitle?: string
    isPublic?: boolean
    featureFlags?: Array<FeatureFlag>
}

export const GroupLayout: (props: Props) => JSX.Element = ({
    tabId,
    themeId,
    entityText,
    image,
    isPublic,
    actions,
    memberStatus,
    routes,
    children,
    shouldRenderGroupHeader = true,
    pageTitle,
    featureFlags,
    ...rest
}) => {
    const router: any = useRouter()
    const isAboutRoute: boolean = router.asPath === routes.groupAboutRoot

    const navMenuList = getGroupNavMenuList({
        groupRoute: routes.groupRoot,
        activeId: tabId,
        isRestricted: isAboutRoute,
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
                            isPublic={isPublic}
                            image={image}
                            themeId={themeId}
                            actions={actions}
                            routes={routes}
                            navMenuList={navMenuList}
                            memberStatus={memberStatus}
                            featureFlags={featureFlags}
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
