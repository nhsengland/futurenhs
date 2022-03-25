import { useRef, useState, useEffect } from 'react';
import classNames from 'classnames';

import { RichText } from '@components/RichText';
import { getAriaFieldAttributes } from '@helpers/util/form';

import { Props } from './interfaces';

export const ImageUpload: (props: Props) => JSX.Element = ({
    input: {
        name,
        value,
        onChange
    },
    initialError,
    meta: {
        touched,
        error,
        submitError
    },
    text,
    relatedFields,
    validators,
    className
}) => {

    const [shouldRenderClearFileInput, setShouldRenderClearFileInput] = useState(false);

    const { label, hint } = text ?? {};

    const id: string = name;
    const shouldRenderError: boolean = Boolean(initialError) || ((Boolean(error) || Boolean(submitError)) && touched);
    const isRequired: boolean = Boolean(validators?.find(({ type }) => type === 'required'));

    const generatedIds: any = {
        hint: `${name}-hint`,
        errorLabel: `${name}-error`,
        clearFile: `${name}-clear`
    };

    const generatedClasses: any = {
        wrapper: classNames('nhsuk-form-group', className, {
            ['nhsuk-form-group--error']: shouldRenderError
        }),
        label: classNames('nhsuk-label'),
        hint: classNames('nhsuk-hint'),
        error: classNames('nhsuk-error-message'),
        input: classNames('nhsuk-input nhsuk-u-width-full u-border-0 u-p-0', {
            ['nhsuk-input--error']: shouldRenderError
        })
    };

    const ariaInputProps: any = getAriaFieldAttributes(isRequired, shouldRenderError, [
        Boolean(hint) ? generatedIds.hint : null,
        shouldRenderError ? generatedIds.errorLabel : null
    ]);

    useEffect(() => {

        const fileIdValue: string = (document.getElementById(relatedFields?.fileId) as any)?.value;

        if(fileIdValue){

            setShouldRenderClearFileInput(true);

        }

    }, []);

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
            <input
                {...ariaInputProps}
                id={id} 
                name={name} 
                type="file" 
                value={value} 
                onChange={onChange}
                className={generatedClasses.input} />
            {shouldRenderClearFileInput &&
                <div className="nhsuk-checkboxes">
                    <div className="nhsuk-checkboxes__item">
                        <input className="nhsuk-checkboxes__input" id={generatedIds.clearFile} name={generatedIds.clearFile} type="checkbox" value="true" />
                        <label className="nhsuk-label nhsuk-checkboxes__label" htmlFor={generatedIds.clearFile}>Clear existing image</label>
                    </div>
                </div>
            }
        </div>

    )

}
