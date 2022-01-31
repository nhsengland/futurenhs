
export const initials = (): Function => {

    return (value: any): String => {

        try {

            const getInitials = (fullName: string): string => {

                const allNames: Array<string> = fullName.trim().split(' ');
                const initials: string = allNames.reduce((acc, curr, index) => {

                    if (index === 0 || index === allNames.length - 1) {
                        acc = `${acc}${curr.charAt(0).toUpperCase()}`;
                    }

                    return acc;

                }, '');

                return initials;

            }

            return getInitials(value);

        } catch (error) {

            return 'An unexpected error occured';

        }

    };

};
