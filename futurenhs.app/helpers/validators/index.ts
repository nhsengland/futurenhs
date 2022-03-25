import { FormField } from '@appTypes/form';

import { required } from './required';
import { email } from './email';
import { maxLength } from './maxLength';
import { maxFileSize } from './maxFileSize';
import { validFileExtensions } from './validFileExtensions';

const validationFunctions: any = {
    required: required,
    email: email,
    maxLength: maxLength,
    maxFileSize: maxFileSize,
    validFileExtensions: validFileExtensions
};

export const validate = (submission: any, fields: Array<FormField>, fieldNameModifier?: string): Record<string, string> => {

    const errors: any = {};

    const recursiveValidator = (fields) => {

        fields?.forEach(({ validators, name, fields: childFields }) => {

            const derivedName: string = fieldNameModifier ? `${name}-${fieldNameModifier}` : name;

            if (validators?.length > 0) {

                for (let i = 0; i < validators.length; i++) {

                    const validator = validators[i];
                    const { type } = validator;

                    const error: string = validationFunctions[type]?.(validator)(submission[derivedName]);

                    if (error) {

                        errors[derivedName] = error;
                        break;

                    }

                }

            }

            recursiveValidator(childFields);

        });

    };

    recursiveValidator(fields);

    return errors;

}
