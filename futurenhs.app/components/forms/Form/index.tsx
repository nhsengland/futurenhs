import { useState, useCallback, useRef, useEffect } from 'react'
import { useRouter } from 'next/router'
import { Form as FinalForm, Field, FormSpy } from 'react-final-form'
import classNames from 'classnames'
import { SVGIcon } from '@components/generic/SVGIcon'
import { Link } from '@components/generic/Link'
import { formComponents } from '@components/forms'
import { Dialog } from '@components/generic/Dialog'
import { validate } from '@helpers/validators'
import { requestMethods } from '@constants/fetch'
import { FormField, FormErrors } from '@appTypes/form'
import { ActionLink } from '@components/generic/ActionLink'
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
    initAction,
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
    shouldRenderSubmitButton = true,
    shouldClearOnSubmitSuccess,
    shouldRenderBackToTopIcon,
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
    const generateFields = () => {
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
    }

    const [fields, setFields] = useState(generateFields())

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
                    shouldRender = true,
                    ...rest
                }) => {
                    if (shouldRender) {
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
                }
            )
        },
        [fields]
    )

    /**
     * Handle generic form life-cycle events
     */
    const handleChange = (props: any): void => {
        changeAction?.(props)
    }
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
     * NB we do a slight workaround here to patch the fact that final-form only deals with
     * json style form submissions, but we need to handle multi-part form submissions with file
     * included
     */
    const handleSubmit = (
        finalFormSubmissionState: Record<string, any>
    ): Promise<FormErrors> => {
        setIsProcessing(true)

        const isFile = (value) => 'File' in window && value instanceof File
        const isBlob = (value) => 'Blob' in window && value instanceof Blob

        /**
         * Iterate through the final form submission data and overwrite the corresponding FormData property
         * UNLESS the FormData property points to a file input stream - in which case leave it alone.
         * This should result in a FormData object which includes both valid file streams and dynamically generated
         * field values
         */
        Object.keys(finalFormSubmissionState).forEach((key) => {
            if (
                submission.current.has(key) &&
                (!isFile(submission.current.get(key)) ||
                    !isBlob(submission.current.get(key)))
            ) {
                submission.current.set(key, finalFormSubmissionState[key])
            }
        })

        /**
         * Submit form data then deal with any errors which come back
         */
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

    useEffect(() => {
        initAction?.(formInstance.current)
    }, [])

    /**
     * Render
     */
    return (
        <FinalForm
            initialValues={initialValues}
            onSubmit={handleSubmit}
            validate={handleValidate}
            render={({ form, errors, handleSubmit, hasValidationErrors }) => {
                formInstance.current = form

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
                            name="_csrf"
                            component={formComponents.hidden}
                            initialValue={csrfToken}
                            defaultValue={csrfToken}
                        />
                        <Field
                            key="_form-id"
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
                                            heading:
                                                'Entered Data will be lost',
                                        }}
                                        cancelAction={handleDiscardFormCancel}
                                        confirmAction={handleDiscardFormConfirm}
                                    >
                                        <p className="u-text-bold">
                                            Any entered details will be
                                            discarded. Are you sure you wish to
                                            proceed?
                                        </p>
                                    </Dialog>
                                </>
                            )}
                            {shouldRenderSubmitButton && (
                                <button
                                    disabled={isProcessing}
                                    type="submit"
                                    className={generatedClasses.submitButton}
                                >
                                    {submitButton}
                                </button>
                            )}
                           

                            {shouldRenderBackToTopIcon && (
                               <div>
                                   <a href="#top" >
                                       <SVGIcon
                                           name="icon-arrow-up"
                                           className="c-svg-icon u-w-4 u-h-14 u-mr-1 u-align-middle"
                                       />
                                       Back to top
                                   </a>
                               </div>
                            )}
                            
                        </div>
                       
                        <FormSpy
                            subscription={{
                                values: true,
                                touched: true,
                                visited: true,
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
