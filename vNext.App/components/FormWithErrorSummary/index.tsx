import { useRef, useState, useCallback, useEffect } from 'react';
import classNames from 'classnames';

import { ErrorSummary } from '@components/ErrorSummary';
import { Form } from '@components/Form';

import { Props } from './interfaces';

export const FormWithErrorSummary: (props: Props) => JSX.Element = ({
    action,
    method = 'POST',
    csrfToken,
    formId,
    instanceId,
    initialValues = {},
    fields,
    errors,
    submitAction,
    cancelHref,
    text,
    children,
    className,
    bodyClassName,
    submitButtonClassName
}) => {

    const errorSummaryRef: any = useRef();
    const [validationErrors, setValidationErrors] = useState(errors ? errors : {});

    const handleChange = useCallback(({ errors, submitErrors, submitFailed }): any => {

        const hasErrors: boolean = errors && Object.keys(errors).length > 0;
        const errorsToUse: Record<string, string> = submitFailed ? hasErrors ? errors : submitErrors : {};

        setTimeout(() => setValidationErrors(errorsToUse), 0);

    }, [errors]);

    const handleSubmitAttempt = () => {

        if(validationErrors){

            errorSummaryRef?.current?.focus();

        }

    };

    const { errorSummary: errorSummaryText, form: formText } = text ?? {};

    const generatedClasses: any = {
        form: classNames('c-form', className)
    };

    useEffect(() => {

        setValidationErrors(errors);

        errorSummaryRef.current?.focus();

    }, [errors]);

    return (

        <>
            <ErrorSummary
                ref={errorSummaryRef}
                errors={validationErrors}
                text={errorSummaryText} 
                className="u-mb-6"/>
                    {children}
            <Form 
                action={action}
                method={method}
                initialValues={initialValues}
                csrfToken={csrfToken}
                formId={formId}
                instanceId={instanceId}
                fields={fields}
                text={formText}
                className={generatedClasses.form}
                bodyClassName={bodyClassName}
                submitButtonClassName={submitButtonClassName}
                cancelHref={cancelHref}
                changeAction={handleChange}
                submitAttemptAction={handleSubmitAttempt}
                submitAction={submitAction} />
        </>
        
    )

}
