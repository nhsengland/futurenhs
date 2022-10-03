import { User } from '@appTypes/user'
import { actions } from '@constants/actions'

export interface Props {
    /**
     * Overrides default list of skip-links for navigating to other areas of a page
     */
    skipLinkList?: Array<{
        id: string
        text: string
    }>
    /**
     * Adds list of links and/or icons to other pages to navigation menu
     */
    navMenuList: Array<{
        url: string
        text: string
        isActive?: boolean
        meta?: {
            themeId?: number | string
            iconName?: string
        }
    }>
    /**
     * Configures text in the user drop-down and log out journey
     */
    text: {
        admin: string
        editProfile: string
        logOut: string
        logOutHeading: string
        logOutBody: string
        logOutCancel: string
        logOutConfirm: string
    }
    /**
     * Data of logged in user, including username. Initials will be generated in the absence of a profile image
     */
    user: User
    actions?: Array<actions>
    /**
     * Controls whether search box should be visible
     */
    shouldRenderSearch?: boolean
    /**
     * Controls whether navigation menu should be visible
     */
    shouldRenderNavigation?: boolean
    /**
     * Stores text typed in search bar
     */
    searchTerm?: any
    className?: string
}
