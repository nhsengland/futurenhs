import { Form as FinalForm, Field, FormSpy } from 'react-final-form';
import classNames from 'classnames';

import { Input } from '@components/Input';
import { HiddenInput } from '@components/HiddenInput';
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
    content,
    className
}) => {

    const handleChange = (props: any): any => changeAction?.(props);
    const handleValidate = (submission: any): any => validate(submission, fields);

    const { submitButtonText } = content ?? {};

    const generatedClasses: any = {
        wrapper: classNames('c-form', className)
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
                    className={generatedClasses.form}>
                        <Field
                            key="_csrf"
                            name="_csrf"
                            component={(HiddenInput as any)} 
                            defaultValue={csrfToken} />
                        {fields.map(({ name, inputType, content }) => {

                            return (

                                <Field
                                    key={name}
                                    name={name}
                                    inputType={inputType}
                                    content={content}
                                    component={(Input as any)} />

                            )

                        })}
                        <button 
                            disabled={submitting}
                            type="submit" 
                            className="c-button c-button--min-width">
                                {submitButtonText}
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
