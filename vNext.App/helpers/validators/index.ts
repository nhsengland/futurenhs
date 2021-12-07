import { Field } from '@appTypes/form';

import { required } from './required';
import { email } from './email';

const validationFunctions: any = {
    required: required,
    email: email
};

export const validate = (submission: any, fields: Array<Field>): any => {

    const errors: any = {};

    fields.forEach(({ name, validators }) => {

        if(validators?.length > 0){

            for(let i = 0; i < validators.length; i++){

                const validator = validators[i]; 
                const { type } = validator;
    
                const error: string = validationFunctions[type](validator)(submission[name]);
    
                if(error){
    
                    errors[name] = error;
                    break;
    
                }
    
            }

        }

    });

    return errors;

}
