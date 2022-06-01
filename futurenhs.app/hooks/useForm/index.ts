import { useContext } from 'react'

import { FormsContext } from '@contexts/index'
import { formTypes } from '@constants/forms'
import { selectForm } from '@selectors/forms'
import { FormConfig, FormErrors } from '@appTypes/form'


export const useFormConfig = (
    formId: formTypes,
    values?: {
        initialValues?: Record<string, any>;
        errors?: FormErrors;
    }
): FormConfig => {
    const config: any = useContext(FormsContext)
    const template: FormConfig = selectForm(config.templates, formId)
    const { initialValues, errors } = values

    if (initialValues) {
        template.initialValues = initialValues
    }

    if (errors) {
        template.errors = errors
    }

    return template
}
