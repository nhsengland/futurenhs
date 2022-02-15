import { FormField } from '@appTypes/form';

import { required } from './required';
import { email } from './email';

const validationFunctions: any = {
    required: required,
    email: email
};

export const validate = (submission: any, fields: Array<FormField>, fieldNameModifier?: string): Record<string, string> => {

    const errors: any = {};

    fields?.forEach(({ name, validators }) => {

        const derivedName: string = fieldNameModifier ? `${name}-${fieldNameModifier}` : name;

        if(validators?.length > 0){

            for(let i = 0; i < validators.length; i++){

                const validator = validators[i]; 
                const { type } = validator;
    
                const error: string = validationFunctions[type](validator)(submission[derivedName]);
    
                if(error){
    
                    errors[derivedName] = error;
                    break;
    
                }
    
            }

        }

    });

    return errors;

}
