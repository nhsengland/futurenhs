import classNames from 'classnames'

import { RichText } from '@components/RichText'
import { RemainingCharacterCount } from '@components/RemainingCharacterCount'
import { getAriaFieldAttributes } from '@helpers/util/form'

import { Props } from './interfaces'

export const Input: (props: Props) => JSX.Element = ({
    inputType,
    input,
    initialError,
    meta: { touched, error, submitError },
    text,
    shouldRenderRemainingCharacterCount,
    validators,
    className,
}) => {

    const { label, hint } = text ?? {}
    const id: string = input.name
    const shouldRenderError: boolean =
        Boolean(initialError) ||
        ((Boolean(error) || Boolean(submitError)) && touched)
    const isRequired: boolean = Boolean(
        validators?.find(({ type }) => type === 'required')
    )
    const maxLength: boolean = validators?.find(
        ({ type }) => type === 'maxLength'
    )?.maxLength

    const generatedIds: any = {
        hint: `${name}-hint`,
        errorLabel: `${name}-error`,
        remainingCharacters: `${name}-remaining-characters`,
    }

    const generatedClasses: any = {
        wrapper: classNames('nhsuk-form-group', className, {
            ['nhsuk-form-group--error']: shouldRenderError,
            ['u-clearfix']: shouldRenderRemainingCharacterCount && maxLength,
        }),
        label: classNames('nhsuk-label'),
        hint: classNames('nhsuk-hint'),
        error: classNames('nhsuk-error-message'),
        input: classNames('nhsuk-input nhsuk-u-width-full', {
            ['nhsuk-input--error']: shouldRenderError,
            ['u-border-0 u-p-0']: inputType === 'file',
        }),
    }

    const ariaInputProps: any = getAriaFieldAttributes(
        isRequired,
        shouldRenderError,
        [
            Boolean(hint) ? generatedIds.hint : null,
            shouldRenderError ? generatedIds.errorLabel : null,
            shouldRenderRemainingCharacterCount
                ? generatedIds.remainingCharacters
                : null,
        ]
    )

    return (
        <div className={generatedClasses.wrapper}>
            <label htmlFor={id} className={generatedClasses.label}>
                {label}
            </label>
            {hint && (
                <RichText
                    id={generatedIds.hint}
                    className={generatedClasses.hint}
                    bodyHtml={hint}
                    wrapperElementType="span"
                />
            )}
            {shouldRenderError && (
                <span className={generatedClasses.error}>
                    {error || submitError || initialError}
                </span>
            )}
            <input
                {...input}
                {...ariaInputProps}
                id={id}
                type={inputType}
                className={generatedClasses.input}
            />
            {shouldRenderRemainingCharacterCount && maxLength && (
                <RemainingCharacterCount
                    id={generatedIds.remainingCharacters}
                    currentCharacterCount={input.value?.length ?? 0}
                    maxCharacterCount={maxLength}
                    className="u-float-right"
                />
            )}
        </div>
    )
}
