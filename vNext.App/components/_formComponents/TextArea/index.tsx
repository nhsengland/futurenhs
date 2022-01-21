import { useRef, useState, useEffect } from 'react';
import classNames from 'classnames';
import { Editor } from '@tinymce/tinymce-react';

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

    const editorRef = useRef(null);
    const [shouldLoadRte, setShouldLoadRte] = useState(false);
    const [isRteFocussed, setIsRteFocussed] = useState(false);

    const { label, hint } = text ?? {};
    const id: string = name;
    const shouldRenderError: boolean = (Boolean(error) || Boolean(submitError)) && touched;

    const handleRteInit = (_, editor) => editorRef.current = editor;
    const handleRteFocus = () => setIsRteFocussed(true);
    const handleRteBlur = () => setIsRteFocussed(false);
    const handleRteChange = () => {

        if (editorRef.current) {

            const rteContent = editorRef.current.getContent();

            onChange(rteContent);

        }

    }

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

    useEffect(() => {

        setShouldLoadRte(true);

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
            {shouldLoadRte
            
                ?   <Editor
                        tinymceScriptSrc="/js/tinymce/tinymce.min.js"
                        textareaName={name}
                        onInit={handleRteInit}
                        onChange={handleRteChange}
                        onFocus={handleRteFocus}
                        onBlur={handleRteBlur}
                        init={{
                            height: 500,
                            menubar: false,
                            plugins: ['autosave link image lists hr anchor wordcount visualblocks visualchars fullscreen media nonbreaking code autolink lists table emoticons charmap'],
                            toolbar: 'undo redo | styleselect| forecolor  | bold italic | alignleft aligncenter alignright alignjustify | outdent indent | link unlink blockquote media image| code table emoticons charmap',
                            content_style: 'body { font-family:Helvetica,Arial,sans-serif; font-size:14px }'
                        }}
                    />

                :   <textarea
                        {...ariaInputProps}
                        id={id}
                        name={name}
                        value={value}
                        onChange={onChange}
                        className={generatedClasses.input} />
            
            }

        </div>

    )

}
