export interface Props {
    linkList: Array<{
        id: string
        /**
         * List of skip links to render including text and id for the target element
         */
        text: string
        className?: string
    }>
}
