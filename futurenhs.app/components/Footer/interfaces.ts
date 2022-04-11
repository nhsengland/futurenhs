export interface Props {
    text: {
        title: string
        navMenuAriaLabel: string
        copyright?: string
    }
    navMenuList: Array<{
        url: string
        text: string
        isActive?: boolean
    }>
    className?: string
}
