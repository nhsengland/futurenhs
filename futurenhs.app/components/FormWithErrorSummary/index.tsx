import { useEffect, useRef, useState } from 'react'
import classNames from 'classnames'

import { ErrorSummary } from '@components/ErrorSummary'
import { Form } from '@components/Form'
import { requestMethods } from '@constants/fetch'
import { FormErrors } from '@appTypes/form'

import { Props } from './interfaces'

export const FormWithErrorSummary: (props: Props) => JSX.Element = ({
    action,
    method = requestMethods.POST,
    csrfToken,
    formConfig,
    instanceId,
    errors,
    context = {},
    submitAction,
    cancelAction,
    cancelHref,
    text,
    children,
    className,
    bodyClassName,
    submitButtonClassName,
    shouldClearOnSubmitSuccess,
}) => {
    const errorSummaryRef: any = useRef()
    const [validationErrors, setValidationErrors] = useState(
        errors ? errors : {}
    )

    /**
     * Handle client-side validation failure in forms
     */
    const handleValidationFailure = (errors: FormErrors): void => {
        setValidationErrors(errors)

        window.scrollTo(0, 0)
        errorSummaryRef?.current?.focus?.()
    }

    /**
     * Handle client-side submit
     */
    const handleSubmit = (formData: FormData): Promise<FormErrors> => {
        setValidationErrors({})

        return submitAction?.(formData)
    }

    const { form: formText } = text ?? {}

    const generatedClasses: any = {
        form: classNames('c-form', className),
    }

    /**
     * Render any errors that happen externally
     */
    useEffect(() => {
        setValidationErrors(errors)
    }, [errors])

    return (
        <>
            <ErrorSummary
                ref={errorSummaryRef}
                errors={validationErrors}
                className="u-mb-6"
            />
            {children}
            <Form
                action={action}
                method={method}
                csrfToken={csrfToken}
                formConfig={formConfig}
                context={context}
                instanceId={instanceId}
                text={formText}
                className={generatedClasses.form}
                bodyClassName={bodyClassName}
                submitButtonClassName={submitButtonClassName}
                shouldClearOnSubmitSuccess={shouldClearOnSubmitSuccess}
                cancelHref={cancelHref}
                cancelAction={cancelAction}
                validationFailAction={handleValidationFailure}
                submitAction={handleSubmit}
            />
        </>
    )
}
