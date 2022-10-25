export interface Props {
    input: {
        name: string
        value: string
        onChange: any
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
    inputType?: 'text' | 'email' | 'password' | 'number' | 'file'
    shouldRenderRemainingCharacterCount?: boolean
    validators?: Array<any>
    className?: string
    autoComplete?: string
}
