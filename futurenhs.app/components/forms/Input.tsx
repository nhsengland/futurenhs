import { VMessage } from '@helpers/validation'
import { ReactNode } from 'react'

export interface Props extends React.HTMLProps<HTMLInputElement> {
    validation?: Array<VMessage>
    label?: string
    hint?: string
    children?: ReactNode
}

export const Input = ({
    validation,
    label,
    hint,
    children,
    ...inputProps
}: Props) => {
    const isError = validation && validation.some((v) => v.error === true)
    const classNames = {
        input: `nhsuk-input nhsuk-u-width-full 
    ${isError ? 'nhsuk-input--error ' : null} 
    ${inputProps.className ?? ''}`,
        hint: `nhsuk-hint ${isError ? 'nhsuk-input--error' : null}`,
        wrapper: `nhsuk-form-group ${
            isError ? 'nhsuk-form-group--error' : null
        }`,
    }

    // const onChange = (e) => {}

    return (
        <div className={classNames.wrapper}>
            {!!hint && <span className={classNames.hint}>{hint}</span>}
            {!!label && <label className="nhsuk-label">{label}</label>}
            {!!isError && (
                <span className="nhsuk-error-message">
                    {validation[0].message}
                </span>
            )}
            <div style={{ position: 'relative' }}>
                {children}
                <input {...inputProps} className={classNames.input} />
            </div>
        </div>
    )
}
