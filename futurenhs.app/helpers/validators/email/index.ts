export const email = (validationMethodData): Function => {

    return (value: string): string => {

    	try {

            const message: string = validationMethodData.message;

	        if (value && typeof value === 'string' && value.length) {

            	const isEmailAddress: boolean = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/.test(value);

            	return isEmailAddress ? undefined : message;

        	}

        	return undefined;

    	} catch(error) {

            console.error(error);

            return 'An unexpected error occured';

    	}

    };

};
