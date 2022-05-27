import { useRef, useState, useEffect } from 'react'
import { Field } from 'react-final-form'
import classNames from 'classnames'

import { RichText } from '@components/RichText'
import { getAriaFieldAttributes } from '@helpers/util/form'

import { Props } from './interfaces'

export const ImageUpload: (props: Props) => JSX.Element = ({
    input: { name, value, onChange },
    initialError,
    meta: { touched, error, submitError },
    text,
    relatedFields,
    validators,
    className,
}) => {
    const fileIdInput: any = useRef(null)
    const initialFileIdValue: any = useRef(null)

    const [shouldRenderClearFileInput, setShouldRenderClearFileInput] =
        useState(false)
    const [shouldDisableClearFileInput, setShouldDisableClearFileInput] =
        useState(false)
    const [shouldClearFile, setShouldClearFile] = useState(false)
    const [hasClearedFile, setHasClearedFile] = useState(false)

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
        clearFile: `${name}-clear`,
    }

    const generatedClasses: any = {
        wrapper: classNames('nhsuk-form-group', className, {
            ['nhsuk-form-group--error']: shouldRenderError,
        }),
        label: classNames('nhsuk-label'),
        hint: classNames('nhsuk-hint'),
        error: classNames('nhsuk-error-message'),
        input: classNames('nhsuk-input nhsuk-u-width-full u-border-0 u-p-0', {
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

    const handleClearFile = (event: any) => {
        event.preventDefault()

        setShouldClearFile(true)
    }

    useEffect(() => {
        fileIdInput.current = document.getElementsByName(
            relatedFields?.fileId
        )[0]
        initialFileIdValue.current = fileIdInput.current.value

        if (initialFileIdValue.current) {
            setShouldRenderClearFileInput(true)
        }
    }, [])

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
                {...ariaInputProps}
                id={id}
                name={name}
                type="file"
                value={value}
                onChange={onChange}
                className={generatedClasses.input}
            />
            {shouldRenderClearFileInput && (
                <button
                    onClick={handleClearFile}
                    className="o-link-button"
                    disabled={shouldDisableClearFileInput}
                >
                    Clear existing image
                </button>
            )}
            {relatedFields?.fileId && (
                <Field name={relatedFields.fileId} subscription={{}}>
                    {({ input: { onChange } }) => {
                        if (shouldClearFile && !hasClearedFile) {
                            setTimeout(() => {
                                onChange('')
                                setHasClearedFile(true)
                                setShouldRenderClearFileInput(false)
                            }, 0)
                        }

                        return null
                    }}
                </Field>
            )}
        </div>
    )
}
