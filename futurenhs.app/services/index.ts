import { services } from '@constants/services'
import { FormErrors } from '@appTypes/form'
export * as services from './'

declare interface ServiceErrorData {
    status: number
    statusText: string
    serviceId: services
    body?: any
}

export class ServiceError extends Error {
    data: ServiceErrorData = {
        status: null,
        statusText: null,
        serviceId: null,
        body: null,
    }

    constructor(message: string, data: ServiceErrorData) {
        super(message)
        this.name = 'ServiceError'
        this.data = data
    }
}

/**
 * Extract the relevant errors to return to the UI on a bad request
 */
export const getServiceErrorDataValidationErrors = (
    error: ServiceError
): FormErrors => {
    if (error.data?.status === 400) {
        /**
         * Field level errors
         */
        if (
            error.data?.body?.errors &&
            Object.keys(error.data.body.errors).length
        ) {
            return error.data.body.errors

            /**
             * Top level error
             */
        } else if (error.data?.body?.error) {
            return {
                _error: error.data?.body?.error,
            }
        }
    }

    return null
}
