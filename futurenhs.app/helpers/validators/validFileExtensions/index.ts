export const validFileExtensions = (validationMethodData: any): Function => {

    /**
     * Intrinsic file type cannot be reliably determined in the browser
     * so this validator is only doing a simple check on file extension
     * It's assumed the server will do a more comprehensive validation check on type!
     */
    return (files: any): string => {

    	try {

		    const message: string = validationMethodData.message;
    		const validFileExtensions: Array<string> = validationMethodData.validFileExtensions;

            let valid: boolean = true;

            if(!message || !validFileExtensions?.length){

                throw new Error('Validator misconfigured');

            }

            if (files) {

                for(let i = 0; i < validFileExtensions.length; i++){

                    validFileExtensions[i] = validFileExtensions[i].toLowerCase();

                }

                for(let i = 0; i < files.length; i++){

                    const file: any = files[i];
                    const { name } = file;

                    if(name && typeof name === 'string'){

                        const test: Array<string> = name.split('.');

                        let extension: string = undefined;

                        /**
                         * Avoid edge cases with hidden files or files with no extension
                         */
                        if(test.length === 1 || (test[0] === '' && test.length === 2)) {

                            extension = '';

                        } else {

                            extension = test.pop();

                        }

                        if(!validFileExtensions.includes(`.${extension}`.toLowerCase())){

                            valid = false;
                            break;

                        }

                    }

                }

        	}

            return valid ? undefined : message;

    	} catch(error) {

            return 'An unexpected error occured';

    	}

    };

};
