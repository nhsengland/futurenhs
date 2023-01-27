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
    inputType?: InputTypes
    shouldRenderRemainingCharacterCount?: boolean
    validators?: Array<any>
    className?: string
    autoComplete?: string
    disabled?: boolean
}

export enum InputTypes {
    USERNAME = 'username',
    PASSWORD = 'password',
    NUMBER = 'number',
    FILE = 'file',
    TEXT = 'text',
    EMAIL = 'email',
}
