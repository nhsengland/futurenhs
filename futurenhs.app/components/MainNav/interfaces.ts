export interface Props {
    navMenuList: Array<{
        url: string
        text: string
        isActive?: boolean
        isActiveRoot?: boolean
        meta?: {
            themeId?: number | string
            iconName?: string
        }
    }>
    className?: string
}
