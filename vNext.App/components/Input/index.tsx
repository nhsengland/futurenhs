import classNames from 'classnames';

import { Props } from './interfaces';

export const Input: (props: Props) => JSX.Element = ({
    inputType,
    input: {
        name,
        value,
        onChange
    },
    meta: {
        touched,
        error,
        submitError
    },
    content,
    className
}) => {

    const { labelText } = content ?? {};
    const id: string = name;
    const shouldRenderError: boolean = (Boolean(error) || Boolean(submitError)) && touched;

    const generatedClasses: any = {
        wrapper: classNames('c-form-group', className, {
            ['c-form-group--error']: shouldRenderError
        }),
        label: classNames('c-label'),
        error: classNames('c-error-message', 'field-validation-error'),
        input: classNames('c-input', 'text-box', 'single-line', {
            ['c-input--error']: shouldRenderError
        })
    };

    return (

        <div className={generatedClasses.wrapper}>
            <label 
                htmlFor={id} 
                className={generatedClasses.label}>
                    {labelText}
            </label>
            {shouldRenderError &&
                <span className={generatedClasses.error}>{error || submitError}</span>
            } 
            <input 
                id={id} 
                name={name} 
                type={inputType} 
                value={value} 
                onChange={onChange}
                className={generatedClasses.input} />
        </div>

    )

}
