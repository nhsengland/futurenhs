
export const matchText = (): Function => {

    return (value: any, term:any): String => {

        try {

            if (value && value.length && term && term.length) {
                const regexPattern : RegExp = new RegExp(`(${term})(?!([^<]+)?>)`, "gi");
                return value.replace(regexPattern,(match)=>`<mark>${match}</mark>`);

            }

            return value;

        } catch(error) {

            return 'An unexpected error occured';

        }

    };

};
