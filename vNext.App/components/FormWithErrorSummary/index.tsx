import { useRef, useState, useCallback } from 'react';
import classNames from 'classnames';

import { ErrorSummary } from '@components/ErrorSummary';
import { Form } from '@components/Form';

import { Props } from './interfaces';

export const FormWithErrorSummary: (props: Props) => JSX.Element = ({
    action,
    method = 'POST',
    csrfToken,
    initialValues = {},
    fields,
    errors,
    submitAction,
    content,
    children,
    className,
    bodyClassName,
    submitButtonClassName
}) => {

    const errorSummaryRef: any = useRef();
    const [validationErrors, setValidationErrors] = useState(errors ? errors : {});
    const relatedNames: Array<string> = fields.map(({ name }) => name);

    const handleChange = useCallback(({ errors, submitErrors, submitFailed, modifiedSinceLastSubmit }): any => {

        const hasErrors: boolean = errors && Object.keys(errors).length > 0;
        const hasSubmitErrors: boolean = submitErrors && Object.keys(submitErrors).length > 0;

        setTimeout(() => {

            setValidationErrors(submitFailed ? hasErrors ? errors : submitErrors : {});

            if(submitFailed && !modifiedSinceLastSubmit && hasSubmitErrors){

                errorSummaryRef.current?.focus();

            }

        }, 0);

    }, [errors]);

    const { errorSummary: errorSummaryContent, form: formContent } = content ?? {};

    const generatedClasses: any = {
        form: classNames('c-form', className)
    };

    return (

        <>
            <ErrorSummary
                ref={errorSummaryRef}
                errors={validationErrors}
                relatedNames={relatedNames}
                content={errorSummaryContent} 
                className="u-mb-6"/>
            {children}
            <Form 
                action={action}
                method={method}
                initialValues={initialValues}
                csrfToken={csrfToken}
                fields={fields}
                content={formContent}
                className={generatedClasses.form}
                bodyClassName={bodyClassName}
                submitButtonClassName={submitButtonClassName}
                changeAction={handleChange}
                submitAction={submitAction} />
        </>
        
    )

}
