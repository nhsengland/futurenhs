import classNames from 'classnames'

import { RichText } from '@components/generic/RichText'
import { getAriaFieldAttributes } from '@helpers/util/form'

import { Props } from './interfaces'

/**
 * Derived from the NHS Design System Checkboxes component: https://service-manual.nhs.uk/design-system/components/checkboxes.
 * Used to let users select 1 or more options on a form.
 */
export const CheckBox: (props: Props) => JSX.Element = ({
    input: { name, value, onChange },
    initialError,
    meta: { touched, error, submitError },
    text,
    validators,
    className,
}) => {
    const { label, hint } = text ?? {}

    const id: string = name
    const shouldRenderError: boolean =
        Boolean(initialError) ||
        ((Boolean(error) || Boolean(submitError)) && touched)
    const isRequired: boolean = Boolean(
        validators?.find(({ type }) => type === 'required')
    )

    const generatedIds: any = {
        hint: `${name}-hint`,
        errorLabel: `${name}-error`,
    }

    const generatedClasses: any = {
        wrapper: classNames('nhsuk-form-group', 'nhsuk-checkboxes', className, {
            ['nhsuk-form-group--error']: shouldRenderError,
        }),
        label: classNames('nhsuk-label nhsuk-checkboxes__label'),
        hint: classNames('nhsuk-hint'),
        error: classNames('nhsuk-error-message'),
        inputWrapper: classNames('nhsuk-checkboxes__item'),
        input: classNames('nhsuk-checkboxes__input', {
            ['nhsuk-input--error']: shouldRenderError,
        }),
    }

    const ariaInputProps: any = getAriaFieldAttributes(
        isRequired,
        shouldRenderError,
        [
            Boolean(hint) ? generatedIds.hint : null,
            shouldRenderError ? generatedIds.errorLabel : null,
        ]
    )

    return (
        <div className={generatedClasses.wrapper}>
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
            <div className={generatedClasses.inputWrapper}>
                <input
                    {...ariaInputProps}
                    id={id}
                    name={name}
                    type="checkbox"
                    value={value}
                    onChange={onChange}
                    className={generatedClasses.input}
                />
                <label htmlFor={id} className={generatedClasses.label}>
                    <RichText bodyHtml={label} />
                </label>
            </div>
        </div>
    )
}
