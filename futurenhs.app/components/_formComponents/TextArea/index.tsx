import { useRef, useState, useEffect } from 'react'
import classNames from 'classnames'
import { Editor } from '@tinymce/tinymce-react'

import { RichText } from '@components/RichText'
import { RemainingCharacterCount } from '@components/RemainingCharacterCount'
import { getAriaFieldAttributes } from '@helpers/util/form'
import { useIntersectionObserver } from '@hooks/useIntersectionObserver'

import { Props } from './interfaces'

export const TextArea: (props: Props) => JSX.Element = ({
    input: { name, value, onChange },
    initialError,
    meta: { touched, error, submitError },
    text,
    shouldRenderAsRte,
    shouldRenderRemainingCharacterCount,
    validators,
    minHeight = 200,
    className,
}) => {
    const editorRef = useRef(null)
    const textAreaRef = useRef(null)

    const entry = useIntersectionObserver(textAreaRef, {
        freezeOnceVisible: true,
    })
    const isVisible =
        !!entry?.isIntersecting || !globalThis.IntersectionObserver

    const [shouldLoadRte, setShouldLoadRte] = useState(false)
    const [isRteFocussed, setIsRteFocussed] = useState(false)

    const { label, hint } = text ?? {}

    const id: string = name
    const shouldRenderError: boolean =
        Boolean(initialError) ||
        ((Boolean(error) || Boolean(submitError)) && touched)
    const isRequired: boolean = Boolean(
        validators?.find(({ type }) => type === 'required')
    )
    const maxLength: boolean = validators?.find(
        ({ type }) => type === 'maxLength'
    )?.maxLength
    const elementMinHeight: string = `${minHeight}px`

    const handleRteInit = (_, editor) => (editorRef.current = editor)
    const handleRteChange = (value: any) => onChange(value)
    const handleRteFocus = () => setIsRteFocussed(true)
    const handleRteBlur = () => setIsRteFocussed(false)

    const generatedIds: any = {
        hint: `${name}-hint`,
        errorLabel: `${name}-error`,
        remainingCharacters: `${name}-remaining-characters`,
    }

    const generatedClasses: any = {
        wrapper: classNames('nhsuk-form-group', className, {
            ['nhsuk-form-group--error']: shouldRenderError,
            ['c-form-group--focus']: isRteFocussed,
            ['u-clearfix']: shouldRenderRemainingCharacterCount && maxLength,
        }),
        label: classNames('nhsuk-label'),
        hint: classNames('nhsuk-hint'),
        error: classNames('nhsuk-error-message'),
        inputWrapper: classNames('nhsuk-textarea-wrapper'),
        input: classNames('nhsuk-textarea', {
            ['u-invisible']: shouldLoadRte,
            ['nhsuk-textarea--error']: shouldRenderError,
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

    useEffect(() => {
        isVisible && shouldRenderAsRte && setShouldLoadRte(true)
    }, [isVisible])

    return (
        <div className={generatedClasses.wrapper}>
            <label htmlFor={id} className={generatedClasses.label}>
                {label}
            </label>
            {hint && (
                <RichText
                    id={generatedIds.hintId}
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
            <div
                className={generatedClasses.inputWrapper}
                style={{ minHeight: elementMinHeight }}
            >
                {shouldLoadRte ? (
                    <Editor
                        tinymceScriptSrc="/js/tinymce/tinymce.min.js"
                        textareaName={name}
                        id={name}
                        value={value}
                        onInit={handleRteInit}
                        onEditorChange={handleRteChange}
                        onFocus={handleRteFocus}
                        onBlur={handleRteBlur}
                        init={{
                            menubar: false,
                            plugins: [
                                'autosave link image lists hr anchor wordcount visualblocks visualchars fullscreen media nonbreaking code autolink lists table emoticons charmap',
                            ],
                            toolbar:
                                'undo redo | styleselect| forecolor  | bold italic | alignleft aligncenter alignright alignjustify | outdent indent | link unlink blockquote media image| code table emoticons charmap',
                            content_style:
                                'body { font-family: Helvetica, Arial, sans-serif; font-size: 19px }',
                            iframe_aria_text:
                                'Rich Text Area. To navigate to the formatting toolbar press OPTION-F10 if you are an Apple user or ALT-F10 for all other users. Make sure your screen reader is in Focus or Auto Forms Mode.',
                        }}
                    />
                ) : (
                    <textarea
                        {...ariaInputProps}
                        ref={textAreaRef}
                        id={id}
                        name={name}
                        value={value}
                        onChange={onChange}
                        className={generatedClasses.input}
                        style={{ minHeight: elementMinHeight }}
                    />
                )}
            </div>
            {shouldRenderRemainingCharacterCount && maxLength && (
                <RemainingCharacterCount
                    id={generatedIds.remainingCharacters}
                    currentCharacterCount={value?.length ?? 0}
                    maxCharacterCount={maxLength}
                    className="u-float-right"
                />
            )}
        </div>
    )
}
