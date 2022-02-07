import { Link } from '@components/Link';
import { Form as FinalForm, Field, FormSpy } from 'react-final-form';
import classNames from 'classnames';

import { formComponents } from '@components/_formComponents';
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
    cancelHref,
    text,
    className,
    bodyClassName,
    submitButtonClassName,
    cancelButtonClassName
}) => {

    const handleChange = (props: any): any => {

        // const { errors, submitFailed } = props;

        // const hasSubmitErrors: boolean = submitFailed && Object.keys(errors).length > 0;

        // if(hasSubmitErrors){

        //     const errorId: string = Object.keys(errors)[0];
        //     const errorField: HTMLElement = document.getElementById(errorId);

        //     errorField?.focus();

        // }

        changeAction?.(props);

    };

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
            render={({ handleSubmit, submitting }) => (
                
                <form 
                    action={action} 
                    method={method}
                    onSubmit={handleSubmit} 
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
                                <Link href={cancelHref}> 
                                    <a className={generatedClasses.cancelButton}>
                                        {cancelButton}
                                    </a>
                                </Link>
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
