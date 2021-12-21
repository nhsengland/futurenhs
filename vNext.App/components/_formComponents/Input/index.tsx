import classNames from 'classnames';

import { RichText } from '@components/RichText';
import { getAriaFieldAttributes } from '@helpers/util/form';

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
    isRequired,
    shouldRenderRemainingCharacterCount,
    className
}) => {

    const { labelText, hintHtml } = content ?? {};
    const id: string = name;
    const shouldRenderError: boolean = (Boolean(error) || Boolean(submitError)) && touched;

    const generatedIds: any = {
        hint: `${name}-hint`,
        errorLabel: `${name}-error`,
        remainingCharacters: `${name}-remaining-characters`
    };

    const generatedClasses: any = {
        wrapper: classNames('c-form-group', className, {
            ['c-form-group--error']: shouldRenderError
        }),
        label: classNames('c-label'),
        hint: classNames('c-hint'),
        error: classNames('c-error-message', 'field-validation-error'),
        input: classNames('c-input', 'text-box', 'single-line', {
            ['c-input--error']: shouldRenderError
        })
    };

    const ariaInputProps: any = getAriaFieldAttributes(isRequired, shouldRenderError, [
        Boolean(hintHtml) ? generatedIds.hint : null,
        shouldRenderError ? generatedIds.errorLabel : null,
        shouldRenderRemainingCharacterCount ? generatedIds.remainingCharacters : null
    ]);

    return (

        <div className={generatedClasses.wrapper}>
            <label 
                htmlFor={id} 
                className={generatedClasses.label}>
                    {labelText}
            </label>
            {hintHtml &&
                <RichText
                    id={generatedIds.hintId}
                    className={generatedClasses.hint}
                    bodyHtml={hintHtml}
                    wrapperElementType="span" />
            }
            {shouldRenderError &&
                <span className={generatedClasses.error}>{error || submitError}</span>
            } 
            <input
                {...ariaInputProps}
                id={id} 
                name={name} 
                type={inputType} 
                value={value} 
                onChange={onChange}
                className={generatedClasses.input} />
        </div>

    )

}
