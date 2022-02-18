import { useRef, useState } from 'react';
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

    /**
     * Handle client-side validation failure in forms
     */
     const handleValidationFailure = (errors): any => {

        setValidationErrors(errors);
        errorSummaryRef?.current?.focus?.();

    };

    /**
     * Handle client-side submit
     */
    const handleSubmit = (body): any => {

        setValidationErrors({});
        submitAction?.(body);

    };

    const { errorSummary: errorSummaryText, form: formText } = text ?? {};

    const generatedClasses: any = {
        form: classNames('c-form', className)
    };

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
                validationFailAction={handleValidationFailure}
                submitAction={handleSubmit} />
        </>
        
    )

}
