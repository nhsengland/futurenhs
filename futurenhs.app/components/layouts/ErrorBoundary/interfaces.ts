export interface Props {
    boundaryId: string
    text?: {
        error?: string
    }
    children?: any
    className?: string
}

export interface State {
    hasError: boolean
}
