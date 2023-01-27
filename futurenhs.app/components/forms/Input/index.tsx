import classNames from 'classnames'
import { RichText } from '@components/generic/RichText'
import { RemainingCharacterCount } from '@components/forms/RemainingCharacterCount'
import { getAriaFieldAttributes } from '@helpers/util/form'
import { InputTypes, Props } from './interfaces'
import { useRef, useState } from 'react'

/**
 * Derived from the NHS Design System Text Input component: https://service-manual.nhs.uk/design-system/components/text-input.
 * Used to allow users to enter text that’s no longer than a single line, such as their name or phone number.
 */
export const Input: (props: Props) => JSX.Element = ({
    inputType,
    input,
    initialError,
    meta: { touched, error, submitError },
    text,
    shouldRenderRemainingCharacterCount,
    validators,
    className,
    autoComplete,
    disabled,
}) => {
    const inputRef = useRef(null)
    const [usernames, setUsernames] = useState<Array<string>>([])
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
        hint: `${id}-hint`,
        errorLabel: `${id}-error`,
        remainingCharacters: `${id}-remaining-characters`,
    }
    const onInput = (
        e: React.KeyboardEvent<HTMLInputElement>,
        inputType: InputTypes
    ) => {
        if (inputType === InputTypes.USERNAME) {
            const isEmail = new RegExp(
                /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
            )
            const val = inputRef.current.value
            if (val.substr(val.length - 1) === ',') {
                inputRef.current.value = ''
                if (!usernames.includes(val))
                    setUsernames([
                        ...usernames,
                        val.substring(0, val.length - 1),
                    ])
            }
        }
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
            ['u-border-0 u-p-0']: inputType === InputTypes.FILE,
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
                autoComplete={autoComplete ? autoComplete : 'off'}
                disabled={disabled}
                ref={inputRef}
                onKeyUp={(e) => {
                    onInput(e, inputType)
                }}
            />
            {usernames}
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
