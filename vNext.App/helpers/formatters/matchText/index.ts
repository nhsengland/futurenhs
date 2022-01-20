
export const matchText = (): Function => {

    return (value: any, term:any): String => {

        try {

            if (value && value.length && term && term.length) {
                var re = new RegExp(term, "gi");
                return value.replace(re,(match)=>`<mark>${match}</mark>`);

            }

            return value;

        } catch(error) {

            return 'An unexpected error occured';

        }

    };

};
