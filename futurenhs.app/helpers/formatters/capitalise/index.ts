export const capitalise = ({ value }): string => {
    if (value && value.length && typeof value === 'string') {
        return value[0].toUpperCase() + value.slice(1)
    }

    return value
}
