export const required = (validationMethodData): Function => {
    return (value: any): string => {
        try {
            const message: string = validationMethodData.message

            if (value) {
                if (Array.isArray(value) && value.length === 0) {
                    return message
                } else if (typeof value === 'string') {
                    if (value.trim && value.trim() === '') {
                        return message
                    }
                }

                return undefined
            }

            return message
        } catch (error) {
            return 'An unexpected error occured'
        }
    }
}
