export interface Props {
    id: string
    /**
     * Controls whether accordion can be clicked
     */
    isDisabled?: boolean
    /**
     * Determines whether accordion/details element is expanded
     */
    isOpen?: boolean
    /**
     * Controls whether accordion should collapse when the user clicks outside of the component
     */
    shouldCloseOnLeave?: boolean
    /**
     * Controls whether accordion should collapse when the user clicks anywhere on the page
     */
    shouldCloseOnContentClick?: boolean
    /**
     * Controls whether accordion should collapse when an anchor tag is clicked
     */
    shouldCloseOnRouteChange?: boolean
    /**
     * Additional function to be called when the accordion is toggled
     */
    toggleAction?: (id: string, isOpen: boolean) => any
    children: any
    /**
     * Text/component to be rendered to control accordion toggle when open
     */
    toggleOpenChildren: any
    /**
     * Text/component to be rendered to control accordion toggle when closed
     */
    toggleClosedChildren: any
    className?: string
    toggleClassName?: string
    contentClassName?: string
}
