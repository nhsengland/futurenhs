import { useContext } from 'react';

import { FormsContext } from '@contexts/index';
import { formTypes } from '@constants/forms'
import { selectForm } from '@selectors/forms'
import { FormConfig, FormErrors } from '@appTypes/form';

export const useFormConfig = (formId: formTypes, initialValues?: Record<string, any>, initialErrors?: FormErrors): FormConfig => {

    const config: any = useContext(FormsContext);
    const template: FormConfig = selectForm(config.templates, formId);

    if(initialValues){

        template.initialValues = initialValues;

    }

    if(initialErrors){

        template.errors = initialErrors;

    }

    return template;
}