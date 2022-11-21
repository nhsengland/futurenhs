export interface Props {
    shouldNoneRole?: boolean
    text: {
        ariaLabel: string
    }
    navMenuList?: Array<{
        url: string
        text: string
        isActive?: boolean
    }>
    shouldFocusActiveLink?: boolean
}
