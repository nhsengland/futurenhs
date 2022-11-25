import { GroupsPageTextContent } from '@appTypes/content'
import { Image } from '@appTypes/image'
import { Routes } from '@appTypes/routing'
import { User } from '@appTypes/user'

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
}

export type GroupTabId = 'index' | 'forum' | 'whiteboard' | 'files' | 'members'
