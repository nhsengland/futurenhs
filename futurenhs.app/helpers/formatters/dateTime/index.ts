import { format, formatDistance, subDays } from 'date-fns'

export const dateTime = ({ value, relativeDayThreshold = 3 }): string => {
    if (value && value.length && typeof value === 'string') {
        try {
            if (
                relativeDayThreshold &&
                subDays(new Date(), relativeDayThreshold) < new Date(value)
            ) {
                return formatDistance(new Date(value), new Date(), {
                    addSuffix: true,
                })
            }

            return format(new Date(value), 'dd MMM yyyy')
        } catch (error) {
            return value
        }
    }

    return value
}
