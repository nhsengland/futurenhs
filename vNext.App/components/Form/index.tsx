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
    text,
    className,
    bodyClassName,
    submitButtonClassName
}) => {

    const handleChange = (props: any): any => changeAction?.(props);
    const handleValidate = (submission: any): any => validate(submission, fields);

    const { submitButton } = text ?? {};

    const generatedClasses: any = {
        wrapper: classNames('c-form', className),
        body: classNames('c-form_body', bodyClassName),
        submitButton: classNames('c-form_submit-button', 'c-button', 'c-button--min-width', submitButtonClassName)
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
                        <button 
                            disabled={submitting}
                            type="submit" 
                            className={generatedClasses.submitButton}>
                                {submitButton}
                        </button>
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
