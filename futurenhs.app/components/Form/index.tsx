import { useState, useCallback, useRef } from 'react'
import { useRouter } from 'next/router'
import { Form as FinalForm, Field, FormSpy } from 'react-final-form'
import classNames from 'classnames'

import { Link } from '@components/Link'
import { formComponents } from '@components/_formComponents'
import { Dialog } from '@components/Dialog'
import { validate } from '@helpers/validators'
import { requestMethods } from '@constants/fetch'
import { FormField, FormErrors } from '@appTypes/form'

import { Props } from './interfaces'

/**
 * Generic form handler
 */
export const Form: (props: Props) => JSX.Element = ({
    action,
    method = requestMethods.POST,
    csrfToken,
    formConfig,
    context = {},
    instanceId,
    changeAction,
    cancelAction,
    submitAction,
    validationFailAction,
    cancelHref,
    text,
    className,
    bodyClassName,
    submitButtonClassName,
    cancelButtonClassName,
    shouldAddErrorTitle = true,
    shouldClearOnSubmitSuccess,
}) => {
    const router = useRouter()

    const submission = useRef(null)
    const formInstance = useRef(null)

    const [isCancelModalOpen, setIsCancelModalOpen] = useState(false)
    const [isProcessing, setIsProcessing] = useState(false)

    const formId: string = formConfig?.id
    const fieldsTemplate: Array<FormField> = formConfig?.steps[0]?.fields
    const initialErrors: FormErrors = formConfig?.errors ?? {}
    const initialValues: Record<string, any> = formConfig?.initialValues ?? {}

    const { submitButton, cancelButton } = text ?? {}

    const shouldRenderCancelButton: boolean =
        Boolean(cancelButton) && (Boolean(cancelHref) || Boolean(cancelAction))

    /**
     * Create unique field instances from the supplied fields template
     */
    const [fields] = useState(() => {
        let templatedFields: Array<FormField>

        try {
            templatedFields = JSON.parse(JSON.stringify(fieldsTemplate))
        } catch (error) {
            templatedFields = []
        }

        templatedFields.forEach((field) => {
            field.name = instanceId ? field.name + '-' + instanceId : field.name
            field.initialError = initialErrors?.[field.name] || null
        })

        return templatedFields
    })

    const generatedClasses: any = {
        wrapper: classNames('c-form', className),
        body: classNames('c-form_body', bodyClassName),
        buttonContainer: classNames('tablet:u-flex', 'u-justify-between'),
        submitButton: classNames(
            'c-form_submit-button',
            'c-button',
            'u-w-full',
            'tablet:u-w-auto',
            'u-mb-4',
            submitButtonClassName
        ),
        cancelButton: classNames(
            'c-form_cancel-button',
            'c-button',
            'c-button-outline',
            'u-w-full',
            'u-mb-4',
            'tablet:u-w-auto',
            cancelButtonClassName
        ),
    }

    /**
     * Recursively render field components from field config
     */
    const renderFields = useCallback(
        (fields?: Array<FormField>): Array<JSX.Element> => {
            if (!fields) {
                return null
            }

            return fields?.map(
                ({
                    name,
                    inputType,
                    text,
                    component,
                    fields,
                    className,
                    ...rest
                }) => {
                    return (
                        <Field
                            key={name}
                            instanceId={instanceId}
                            name={name}
                            inputType={inputType}
                            type={inputType}
                            text={text}
                            component={formComponents[component]}
                            context={context}
                            className={className}
                            {...rest}
                        >
                            {renderFields(fields)}
                        </Field>
                    )
                }
            )
        },
        [fields]
    )

    /**
     * Handle generic form life-cycle events
     */
    const handleChange = (props: any): void => changeAction?.(props)
    const handleValidate = (submission: any): Record<string, string> =>
        validate(submission, fields)
    const handleValidationFailure = (errors: any) => {
        validationFailAction?.(errors)
        setPageTitle(errors)
    }

    /**
     * Applies the error pattern to the page title
     */
    const setPageTitle = (errors: FormErrors): void => {
        const basePageTitle: string = document.title.replace('Error: ', '')

        if (shouldAddErrorTitle && errors && Object.keys(errors).length > 0) {
            document.title = `Error: ${basePageTitle}`
        } else {
            document.title = basePageTitle
        }
    }

    /**
     * Handle a form submission and return an object of external errors if any occur
     * NB *ignore* the submission data sent as an arg to this handler and use the submission saved in the ref
     * as only this supports multi-part form data
     */
    const handleSubmit = (_): Promise<FormErrors> => {
        setIsProcessing(true)

        return submitAction?.(submission.current)?.then(
            (errors: FormErrors) => {
                setIsProcessing(false)

                if (!errors || Object.keys(errors).length === 0) {
                    /**
                     * Clear the form if the submission completed without errors
                     */
                    shouldClearOnSubmitSuccess && formInstance.current.restart()
                } else {
                    handleValidationFailure(errors)
                }

                return errors
            }
        )
    }

    /**
     * Render
     */
    return (
        <FinalForm
            initialValues={initialValues}
            onSubmit={handleSubmit}
            validate={handleValidate}
            render={({ form, errors, handleSubmit, hasValidationErrors }) => {
                /**
                 * Handles opening a modal to confirm cancellation of a submission
                 */
                const handleCancel = (event: any): any => {
                    event.preventDefault()

                    setIsCancelModalOpen(true)
                }

                /**
                 * Handles discarding the submission cancellation
                 */
                const handleDiscardFormCancel = (): void =>
                    setIsCancelModalOpen(false)

                /**
                 * Handles the submission cancellation
                 */
                const handleDiscardFormConfirm = (): Promise<boolean> => {
                    form.restart()

                    if (cancelHref) {
                        return router.push(cancelHref)
                    } else if (cancelAction) {
                        setIsCancelModalOpen(false)
                        cancelAction()
                    }
                }

                return (
                    <form
                        action={action}
                        method={method}
                        encType="multipart/form-data"
                        noValidate={true}
                        onSubmit={async (event: any) => {
                            event.preventDefault()

                            if (!isProcessing) {
                                /**
                                 * Handle client-side validation failure in forms
                                 */
                                if (hasValidationErrors) {
                                    handleValidationFailure(errors)
                                }

                                /**
                                 * We need to handle multi-part forms using a FormData object to support file uploads
                                 * react-final-form only natively handles submissions as JSON in handleSubmit, so the FormData is cached in a ref before calling handleSubmit
                                 */
                                submission.current = new FormData(event.target)
                                formInstance.current = form

                                /**
                                 * Submit and then reset the form on success
                                 */
                                handleSubmit()
                            }
                        }}
                        className={generatedClasses.wrapper}
                    >
                        <Field
                            key="_csrf"
                            id={`_csrf${instanceId ?? ''}`}
                            name="_csrf"
                            component={formComponents.hidden}
                            initialValue={csrfToken}
                            defaultValue={csrfToken}
                        />
                        <Field
                            key="_form-id"
                            id={`_form-id${instanceId ?? ''}`}
                            name="_form-id"
                            component={formComponents.hidden}
                            initialValue={formId}
                            defaultValue={formId}
                        />
                        {instanceId && (
                            <Field
                                key="_instance-id"
                                id={`_instance-id${instanceId}`}
                                name="_instance-id"
                                component={formComponents.hidden}
                                initialValue={instanceId}
                                defaultValue={instanceId}
                            />
                        )}
                        <div className={generatedClasses.body}>
                            {renderFields(fields)}
                        </div>
                        <div className={generatedClasses.buttonContainer}>
                            {shouldRenderCancelButton && (
                                <>
                                    {cancelHref && (
                                        <Link href={cancelHref}>
                                            <a
                                                className={
                                                    generatedClasses.cancelButton
                                                }
                                                onClick={handleCancel}
                                            >
                                                {cancelButton}
                                            </a>
                                        </Link>
                                    )}
                                    {cancelAction && (
                                        <button
                                            className={
                                                generatedClasses.cancelButton
                                            }
                                            onClick={handleCancel}
                                        >
                                            {cancelButton}
                                        </button>
                                    )}
                                    <Dialog
                                        id="dialog-discard-discussion"
                                        isOpen={isCancelModalOpen}
                                        text={{
                                            cancelButton: 'Cancel',
                                            confirmButton: 'Yes, discard',
                                        }}
                                        cancelAction={handleDiscardFormCancel}
                                        confirmAction={handleDiscardFormConfirm}
                                    >
                                        <h3>Entered Data will be lost</h3>
                                        <p className="u-text-bold">
                                            Any entered details will be
                                            discarded. Are you sure you wish to
                                            proceed?
                                        </p>
                                    </Dialog>
                                </>
                            )}
                            <button
                                disabled={isProcessing}
                                type="submit"
                                className={generatedClasses.submitButton}
                            >
                                {submitButton}
                            </button>
                        </div>
                        <FormSpy
                            subscription={{
                                touched: true,
                                errors: true,
                                submitErrors: true,
                                submitFailed: true,
                                submitSucceeded: true,
                                modifiedSinceLastSubmit: true,
                            }}
                            onChange={handleChange}
                        />
                    </form>
                )
            }}
        />
    )
}
