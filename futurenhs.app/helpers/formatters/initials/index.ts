export const initials = ({
    value
}): string => {

    if (value && value.length && typeof value === 'string') {

        const getInitials = (fullName: string): string => {

            if (fullName) {

                const allNames: Array<string> = fullName.trim().split(' ');
                const initials: string = allNames.reduce((acc, curr, index) => {

                    if (index === 0 || index === allNames.length - 1) {
                        acc = `${acc}${curr.charAt(0).toUpperCase()}`;
                    }

                    return acc;

                }, '');

                return initials;

            }

            return '';

        }

        return getInitials(value);

    }

    return value;

};
