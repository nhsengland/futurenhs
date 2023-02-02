export interface Props {
    input: {
        name: string
        value: any
        onChange: any
        onFocus: any
        onBlur
    }
    meta: {
        error: string
        submitError: string
        touched: boolean
    }
    initialError?: string
    text: {
        label: string
        hint?: string
        error?: string
    }
    shouldRenderAsRte?: boolean
    shouldRenderRemainingCharacterCount?: boolean
    rteToolBarOptions?: string
    validators?: Array<any>
    minHeight?: number
    className?: string
}
