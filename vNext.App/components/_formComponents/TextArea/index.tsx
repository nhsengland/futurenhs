import classNames from 'classnames';

import { RichText } from '@components/RichText';
import { getAriaFieldAttributes } from '@helpers/util/form';

import { Props } from './interfaces';

export const TextArea: (props: Props) => JSX.Element = ({
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
    text,
    isRequired,
    shouldRenderRemainingCharacterCount,
    className
}) => {

    const { label, hint } = text ?? {};
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
        input: classNames('c-textarea', {
            ['c-input--error']: shouldRenderError
        })
    };

    const ariaInputProps: any = getAriaFieldAttributes(isRequired, shouldRenderError, [
        Boolean(hint) ? generatedIds.hint : null,
        shouldRenderError ? generatedIds.errorLabel : null,
        shouldRenderRemainingCharacterCount ? generatedIds.remainingCharacters : null
    ]);

    return (

        <div className={generatedClasses.wrapper}>
            <label 
                htmlFor={id} 
                className={generatedClasses.label}>
                    {label}
            </label>
            {hint &&
                <RichText
                    id={generatedIds.hintId}
                    className={generatedClasses.hint}
                    bodyHtml={hint}
                    wrapperElementType="span" />
            }
            {shouldRenderError &&
                <span className={generatedClasses.error}>{error || submitError}</span>
            } 
            <textarea 
                id={id} 
                name={name} 
                value={value} 
                onChange={onChange}
                className={generatedClasses.input} />
        </div>

    )

}
