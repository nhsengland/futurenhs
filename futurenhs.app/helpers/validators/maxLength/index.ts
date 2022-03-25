export const maxLength = (validationMethodData): Function => {

    return (value: any): string => {

    	try {

		    const message: string = validationMethodData.message;
            const maxLength: number = validationMethodData.maxLength;

	        if (value && typeof value === 'string' && value.length) {

            	const inputValueLength: number = value.trim().length;

            	return inputValueLength > maxLength ? message : undefined;

        	}

            return undefined;

    	} catch(error) {

            console.error(error);

            return 'An unexpected error occured';

    	}

    };

};
