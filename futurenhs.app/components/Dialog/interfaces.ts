export interface Props {
    id: string
    /**
     * Determines whether dialog modal is visible
     */
    isOpen?: boolean
    /**
     * Renders elements inside the dialog, e.g headings, confirmation messages, warnings etc.
     */
    children?: any
    appElement?: HTMLElement
    /**
     * Controls text displayed on confirmation and cancel buttons
     */
    text: {
        confirmButton: string
        cancelButton?: string
    }
    /**
     * Function to be called upon selecting confirm button
     */
    confirmAction?: any
    /**
     * Function to be called upon selecting cancel button
     */
    cancelAction?: any
    className?: string
}
