export interface Props {
    /**
     * Controls text content and accessible aria-label
     */
    text: {
        title: string
        navMenuAriaLabel: string
        copyright?: string
    }
    /**
     * Creates a list of navigation links
     */
    navMenuList: Array<{
        url: string
        text: string
        isActive?: boolean
    }>
    className?: string
}
