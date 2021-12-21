export const capitalise = (): Function => {

    return (value: any): string => {

        try {

            if (value && value.length) {

                return value[0].toUpperCase() + value.slice(1);

            }

            return value;

        } catch(error) {

            return 'An unexpected error occured';

        }

    };

};
