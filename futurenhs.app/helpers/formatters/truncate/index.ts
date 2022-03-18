
export const truncate = ({
    value,
    limit,
    suffix = '...'
}): string => {

    if (value?.length && typeof value === 'string') {

        let truncated: string = value.replace(/(<([^>]+)>)/gi, "");

        if (truncated.length > limit) {

            truncated = truncated.substring(0, limit);

            return truncated + suffix;

        }

        return truncated;

    }

    return value;

};
