import { actions } from '@constants/actions'
import { Image } from '@appTypes/image'
import { Routes } from '@appTypes/routing'

export interface Props {
    id: string
    /**
     * Controls the colour scheme
     */
    themeId?: string
    /**
     * Adds an image to the header
     */
    image?: Image
    /**
     * Configures visible heading and description. navMenuTitle is added to navigation bar as an aria-label
     */
    text?: {
        mainHeading: string
        description?: string
        navMenuTitle: string
    }
    isPublic?: boolean
    isDiscover?: boolean
    /**
     * Adds list of links to navigation menu
     */
    navMenuList?: Array<{
        url: string
        text: string
        isActive?: boolean
    }>
    routes?: Routes
    /**
     * Determines which options should be rendered e.g join group, edit group, leave group etc.
     */
    actions?: Array<actions>
    /**
     * Controls whether actions menu should be displayed
     */
    shouldRenderActionsMenu?: boolean
    className?: string
    memberStatus?: string
}
