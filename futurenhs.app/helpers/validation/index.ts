export interface ValidationMessage {
    success: boolean
    name: string
    message: string | null
}

export const approve = (name): ValidationMessage => ({
    success: true,
    name,
    message: null,
})
export const deny = (name, message): ValidationMessage => ({
    success: false,
    name,
    message,
})
