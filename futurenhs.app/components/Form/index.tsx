import { useState, useCallback } from 'react';
import { useRouter } from 'next/router';
import { Link } from '@components/Link';
import { Form as FinalForm, Field, FormSpy } from 'react-final-form';
import classNames from 'classnames';

import { formComponents } from '@components/_formComponents';
import { Dialog } from '@components/Dialog';
import { validate } from '@helpers/validators';
import { FormField } from '@appTypes/form';

import { Props } from './interfaces';

/**
 * Generic form handler
 */
export const Form: (props: Props) => JSX.Element = ({
    action,
    method = 'POST',
    csrfToken,
    formId,
    instanceId,
    initialValues = {},
    fields: fieldsTemplate,
    changeAction,
    submitAction,
    validationFailAction,
    cancelHref,
    text,
    className,
    bodyClassName,
    submitButtonClassName,
    cancelButtonClassName
}) => {

    const router = useRouter();

    /**
     * Create unique field instances from the supplied fields template
     */
    const [fields] = useState(() => {

        let templatedFields: Array<FormField>;
        
        try {

            templatedFields = JSON.parse(JSON.stringify(fieldsTemplate));

        } catch(error){

            templatedFields = [];

        }
        
        templatedFields.forEach(field => {
        
            field.name = instanceId ? field.name + '-' + instanceId : field.name;  
        
        });
        
        return templatedFields;

    });
    
    const [isCancelModalOpen, setIsCancelModalOpen] = useState(false);
    const handleCancel = (event: any): any => {

        event.preventDefault();

        setIsCancelModalOpen(true);

    };
    const handleDiscardFormCancel = (): void => setIsCancelModalOpen(false);
    const handleDiscardFormConfirm = (): Promise<boolean> => router.push(cancelHref);
    const handleChange = (props: any): void => changeAction?.(props);
    const handleValidate = (submission: any): Record<string, string> => validate(submission, fields);
    const handleSubmit = (submission: any): void => {

        const formData: FormData = new FormData();

        for(const fieldName in submission){

            formData.append(fieldName, submission[fieldName]);

        }
        
        submitAction?.(formData);
        
    };

    const { submitButton, cancelButton } = text ?? {};

    const shouldRenderCancelButton: boolean = Boolean(cancelButton) && Boolean(cancelHref);

    const generatedClasses: any = {
        wrapper: classNames('c-form', className),
        body: classNames('c-form_body', bodyClassName),
        buttonContainer: classNames('u-flex', 'u-justify-between'),
        submitButton: classNames('c-form_submit-button', 'c-button', 'c-button--min-width', submitButtonClassName),
        cancelButton: classNames('c-form_cancel-button', 'c-button', 'c-button-outline', 'c-button--min-width', cancelButtonClassName)
    };

    const hasUploadFields = useCallback((fields?: Array<FormField>): boolean => {

        let hasUploadFields: boolean = false;

        if(!fields){

            return hasUploadFields;

        }

        const iterator = (fields: Array<FormField>) => {

            fields?.forEach(({ component, inputType, fields: childFields }) => {

                if(component === 'input' && inputType === 'file'){

                    hasUploadFields = true;

                }

                iterator(childFields);

            })

        };

        iterator(fields);

        return hasUploadFields;

    }, [fields]);
    
    const renderFields = useCallback((fields?: Array<FormField>): Array<JSX.Element> => {

        if(!fields){

            return null;

        }

        return fields?.map(({ 
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
                    text={text}
                    component={formComponents[component]}
                    className={className}
                    {...rest}>
                        {renderFields(fields)}
                </Field>

            )

        });

    }, [fields]);

    const encType: string = hasUploadFields(fields) ? 'multipart/form-data' : null;

    return (

        <FinalForm
            initialValues={initialValues}
            onSubmit={handleSubmit}
            validate={handleValidate}
            render={({ 
                form,
                errors,
                hasValidationErrors,
                handleSubmit, 
                submitting 
            }) => (
                
                <form 
                    action={action} 
                    method={method}
                    encType="multipart/form-data"
                    onSubmit={(event: any) => {

                        event.preventDefault();

                        /**
                         * Handle client-side validation failure in forms
                         */
                        if(hasValidationErrors){

                            validationFailAction?.(errors);

                        }

                        /**
                         * Submit and then reset the form on success
                         */
                        handleSubmit()?.then(() => form.restart());

                    }} 
                    className={generatedClasses.wrapper}>
                        <Field
                            key="_csrf"
                            id={`_csrf${instanceId}`}
                            name="_csrf"
                            component={formComponents.hidden} 
                            defaultValue={csrfToken} />
                        <Field
                            key="_form-id"
                            id={`_form-id${instanceId}`}
                            name="_form-id"
                            component={formComponents.hidden} 
                            defaultValue={formId} />
                        {instanceId &&
                            <Field
                                key="_instance-id"
                                id={`_instance-id${instanceId}`}
                                name="_instance-id"
                                component={formComponents.hidden} 
                                defaultValue={instanceId} />
                        }
                        <div className={generatedClasses.body}>
                            {renderFields(fields)}
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
                                        cancelAction={handleDiscardFormCancel}
                                        confirmAction={handleDiscardFormConfirm}>
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
                                touched: true,
                                errors: true,
                                submitErrors: true,
                                submitFailed: true, 
                                submitSucceeded: true,
                                modifiedSinceLastSubmit: true
                            }}
                            onChange={handleChange}
                        />
                </form>

            )}
        />
    )
    

}
