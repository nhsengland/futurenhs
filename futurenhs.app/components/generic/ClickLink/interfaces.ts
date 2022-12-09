export interface Props {
    /**
     * Controls visible link text and aria label to provide more context
     */
    text: {
        body: string
        ariaLabel: string
    }
    onClick?: React.MouseEventHandler<HTMLDivElement>
    material?: true
    /**
     * Controls which icon to render
     */
    iconName?: string
    className?: string
}
