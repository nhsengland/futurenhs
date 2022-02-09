import { useState } from 'react';
import { useRouter } from 'next/router';
import { Link } from '@components/Link';
import { Form as FinalForm, Field, FormSpy } from 'react-final-form';
import classNames from 'classnames';

import { formComponents } from '@components/_formComponents';
import { Dialog } from '@components/Dialog';
import { validate } from '@helpers/validators';

import { Props } from './interfaces';

export const Form: (props: Props) => JSX.Element = ({
    action,
    method = 'POST',
    csrfToken,
    initialValues = {},
    fields,
    changeAction,
    submitAction,
    submitAttemptAction,
    cancelHref,
    text,
    className,
    bodyClassName,
    submitButtonClassName,
    cancelButtonClassName
}) => {

    const router = useRouter();
    const [isCancelModalOpen, setIsCancelModalOpen] = useState(false);

    const handleCancel = (event: any): any => {

        event.preventDefault();

        setIsCancelModalOpen(true);

    };
    const handleDiscardDiscussionCancel = () => setIsCancelModalOpen(false);
    const handleDiscardDiscussionConfirm = () => router.push(cancelHref);
    const handleChange = (props: any): any => changeAction?.(props);
    const handleValidate = (submission: any): any => validate(submission, fields);

    const { submitButton, cancelButton } = text ?? {};

    const shouldRenderCancelButton: boolean = Boolean(cancelButton) && Boolean(cancelHref);

    const generatedClasses: any = {
        wrapper: classNames('c-form', className),
        body: classNames('c-form_body', bodyClassName),
        buttonContainer: classNames('u-flex', 'u-justify-between'),
        submitButton: classNames('c-form_submit-button', 'c-button', 'c-button--min-width', submitButtonClassName),
        cancelButton: classNames('c-form_cancel-button', 'c-button', 'c-button-outline', 'c-button--min-width', cancelButtonClassName)
    };

    return (

        <FinalForm
            initialValues={initialValues}
            onSubmit={submitAction}
            validate={handleValidate}
            render={({ 
                handleSubmit, 
                submitting 
            }) => (
                
                <form 
                    action={action} 
                    method={method}
                    onSubmit={(event: any) => {

                        event.preventDefault();

                        submitAttemptAction?.();
                        handleSubmit();

                    }} 
                    className={generatedClasses.wrapper}>
                        <Field
                            key="_csrf"
                            name="_csrf"
                            component={formComponents.hidden} 
                            defaultValue={csrfToken} />
                        <div className={generatedClasses.body}>
                            {fields?.map(({ 
                                name, 
                                inputType, 
                                text, 
                                component,
                                className 
                            }) => {

                                return (

                                    <Field
                                        key={name}
                                        name={name}
                                        inputType={inputType}
                                        text={text}
                                        component={formComponents[component]}
                                        className={className} />

                                )

                            })}
                        </div>
                        <div className={generatedClasses.buttonContainer}>
                            {shouldRenderCancelButton &&
                                <>
                                    <Link href={cancelHref}> 
                                        <a className={generatedClasses.cancelButton} onClick={handleCancel}>
                                            {cancelButton}
                                        </a>
                                    </Link>
                                    <Dialog 
                                        id="dialog-discard-discussion"
                                        isOpen={isCancelModalOpen}
                                        text={{
                                            cancelButton: 'Cancel',
                                            confirmButton: 'Yes, discard'
                                        }}
                                        cancelAction={handleDiscardDiscussionCancel}
                                        confirmAction={handleDiscardDiscussionConfirm}>
                                            <h3>Entered Data will be lost</h3>
                                            <p className="u-text-bold">Any entered details will be discarded. Are you sure you wish to proceed?</p>
                                    </Dialog>
                                </>
                            }
                            <button 
                                disabled={submitting}
                                type="submit" 
                                className={generatedClasses.submitButton}>
                                    {submitButton}
                            </button>
                        </div>
                        <FormSpy
                            subscription={{ 
                                errors: true,
                                submitErrors: true,
                                submitFailed: true, 
                                modifiedSinceLastSubmit: true
                            }}
                            onChange={handleChange}
                        />
                </form>

            )}
        />
    )
    

}
