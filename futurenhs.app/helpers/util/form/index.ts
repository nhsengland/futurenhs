const FormData = require('form-data')

import { genericMessages } from '@constants/text'
import { FormConfig, FormErrors, FormOptions } from '@appTypes/form'

/**
 * Converts a form submission object submitted via express-form-data into a basic server-side FormData object
 * which exposes get methods that reflect those of FormData for use in services to that standard url encoded and multipart form requests
 * can be handled more generically
 */
export interface ServerSideFormData {
    get: (fieldName: string) => any
    getAll: () => Record<string, any>
}

export const getServerSideFormData = (
    body: Record<any, any>
): ServerSideFormData => {
    class PseudoFormData {
        body

        constructor(body) {
            this.body = body
        }

        public get(fieldName) {
            return this.body[fieldName]
        }

        public getAll() {
            return this.body
        }
    }

    return new PseudoFormData(body)
}

/**
 * Converts a form submission object submitted via express-form-data into a server-side FormData object
 * for submission to services
 */
export const getServerSideMultiPartFormData = (body: Record<any, any>): any => {
    const formData: any = new FormData()

    for (const fieldName in body) {
        formData.append(fieldName, body[fieldName])
    }

    return formData
}

/**
 * Returns relevant aria attributes for a field's current state
 */
export const getAriaFieldAttributes = (
    isRequired: boolean,
    isError: boolean,
    describedBy?: Array<string>,
    labelledBy?: string
): any => {
    let ariaProps: any = {}

    if (isRequired) {
        ariaProps['aria-required'] = 'true'
    }

    if (isError) {
        ariaProps['aria-invalid'] = 'true'
    }

    if (describedBy?.length) {
        let ids: string = ''

        describedBy.forEach((id: string) => {
            if (id) {
                ids += `${ids ? ' ' : ''}${id}`
            }
        })

        if (ids) {
            ariaProps['aria-describedby'] = ids
        }
    }

    if (labelledBy) {
        ariaProps['aria-labelledby'] = labelledBy
    }

    return ariaProps
}

/**
 * Returns a generic top level form submission error object
 */
export const getGenericFormError = (error: any): FormErrors => {
    return {
        _error: error.message || genericMessages.UNEXPECTED_ERROR,
    }
}

/**
 * Returns a new form config object with new list of options
 */
export const setFormConfigOptions = (
    formConfig: FormConfig,
    step = 0,
    name: string,
    options: Array<FormOptions>
): FormConfig => {
    const configCopy = JSON.parse(JSON.stringify(formConfig))

    const targetField = configCopy.steps[step].fields.find(
        (field) => field.name === name
    )

    targetField.options = options

    return configCopy
}

/**
 * Checks if server side form submission is the expected form
 */
export const checkMatchingFormType = (
    formData: Record<any, any>,
    formId: string
): boolean => {
    return Boolean(formData.body?.['_form-id'] === formId)
}
