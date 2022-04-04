import classNames from 'classnames';
import { RichText } from '@components/RichText';

import { Props } from './interfaces';

/**
 * Multi-choice component for use with Redux Form
 */
export const MultiChoice: Function = ({
    inputType,
    meta: {
        touched,
        error,
        submitError
    },
    initialError,
    input: {
        name,
        value,
        onBlur,
        onChange,
    },
    options = [],
    isDisabled,
    text: {
        label,
        hint
    },
    className,
    optionClassName
}: Props): JSX.Element => {

    /**
     * Render out checkboxes/ radio buttons and bind event handler to update values
     */
    const renderInputs: Function = (options: Array<any>): any => {

        const hasError: boolean = touched && Boolean(error);
        const currentValues: Array<any> = value && Array.isArray(value) ? value : [value];

        return (

            <>

                {options.map(({ label, value }, index) => {

                    const checkbox: string = 'checkbox';
                    const radio: string = 'radio';

                    // https://github.com/erikras/redux-form/issues/2880
                    const elementType: string = inputType === 'checkBox' ? checkbox : radio;
                    const childId: string = `${name}[${index}]`;
                    const childName: string = inputType === checkbox ? childId : name;
                    const isChecked: boolean = currentValues.length && currentValues.includes(value);

                    const generatedClasses: any = {
                        inputWrapper: classNames('nhsuk-radios__item', optionClassName),
                        input: classNames('nhsuk-radios__input', {
                            ['nhsuk-radios__input--validation-failed']: hasError
                        }),
                        label: classNames('nhsuk-label', `nhsuk-radios__label`)
                    };

                    const handleBlur = (): Function => onBlur(currentValues);
                    const handleChange = (event: React.ChangeEvent<HTMLInputElement>): Function => {

                        const updatedValues: Array<any> = elementType === checkbox ? [...currentValues] : [];
                        const isChecked: boolean = event.target.checked;

                        if (isChecked) {

                            updatedValues.push(value);

                        } else {

                            updatedValues.splice(updatedValues.indexOf(value), 1);

                        }

                        return onChange(updatedValues);

                    };

                    return (

                        <div key={childId} className={generatedClasses.inputWrapper}>
                            <input
                                id={childId}
                                name={childName}
                                type={elementType}
                                value={value}
                                checked={isChecked}
                                disabled={isDisabled}
                                onChange={handleChange}
                                onBlur={handleBlur}
                                className={generatedClasses.input} />
                            <label htmlFor={childId} className={generatedClasses.label}>
                                <RichText bodyHtml={label} />
                            </label>
                        </div>

                    );


                })}

            </>

        );

    }
    
    const hasError: boolean = Boolean(initialError) || touched && Boolean(error);
    const shouldRenderErrors: boolean = hasError;

    const generatedIds: any = {
        hint: `${name}-hint`,
        errorLabel: `${name}-error`
    };

    const generatedClasses: any = {
        wrapper: classNames('nhsuk-form-group', 'nhsuk-radios', 'nhsuk-radios--inline', className, {
            ['nhsuk-form-group--error']: hasError
        }),
        fieldset: classNames('nhsuk-fieldset'),
        legend: classNames('nhsuk-fieldset__legend'),
        hint: classNames('nhsuk-hint'),
        error: classNames('nhsuk-error-message')
    };

    /**
     * Else render the component
     */
    return (

        <div className={generatedClasses.wrapper}>
            <fieldset className={generatedClasses.fieldset}>
                <legend id={name} className={generatedClasses.legend}>
                    <RichText bodyHtml={label} />
                </legend>
                {hint &&
                    <RichText
                        id={generatedIds.hintId}
                        className={generatedClasses.hint}
                        bodyHtml={hint}
                        wrapperElementType="span" />
                }
                {shouldRenderErrors &&
                    <span className={generatedClasses.error}>{error || submitError || initialError}</span>
                }
                {renderInputs(options)}
            </fieldset>
        </div>

    );
}
