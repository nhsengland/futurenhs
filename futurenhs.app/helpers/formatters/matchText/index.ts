export const matchText = ({ value, term }): string => {
    if (
        value &&
        value.length &&
        typeof value === 'string' &&
        term &&
        term.length &&
        typeof term === 'string'
    ) {
        const regexPattern: RegExp = new RegExp(`(${term})(?!([^<]+)?>)`, 'gi')

        return value.replace(regexPattern, (match) => `<mark>${match}</mark>`)
    }

    return value
}
