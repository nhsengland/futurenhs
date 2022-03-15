import { useState, useRef, useEffect } from 'react';
import classNames from 'classnames';

import { formTypes } from '@constants/forms';
import { Form } from '@components/Form';
import { Accordion } from '@components/Accordion';
import { SVGIcon } from '@components/SVGIcon';
import { selectFormDefaultFields } from '@selectors/forms';
import forms from '@formConfigs/index';
import { FormErrors } from '@appTypes/form';

import { Props } from './interfaces';

export const Reply: (props: Props) => JSX.Element = ({
    targetId,
    csrfToken,
    initialErrors,
    text,
    validationFailAction,
    submitAction,
    className
}) => {

    const wrapperRef = useRef();

    const fields = selectFormDefaultFields(forms, formTypes.CREATE_DISCUSSION_COMMENT_REPLY);

    const [isReplyAccordionOpen, setIsReplyAccordionOpen] = useState(false);
    const [shouldRenderCancelButton, setShouldRenderCancelButton] = useState(false);

    const { reply } = text;

    const handleToggle = ((_, isOpen) => {

        setIsReplyAccordionOpen(isOpen);

    });

    const handleCancel = shouldRenderCancelButton ? ((): void => {

        const summaryElement: HTMLElement = (wrapperRef.current as any).getElementsByTagName('SUMMARY')?.[0] as HTMLElement;

        setIsReplyAccordionOpen(false);
        summaryElement?.focus();

    }) : null;

    const handleSubmit = ((formData: FormData): Promise<FormErrors> => {

        setIsReplyAccordionOpen(false);

        return submitAction(formData);

    });

    const generatedIds: any = {
        accordion: `${targetId}-reply-accordion`
    };

    const generatedClasses: any = {
        wrapper: classNames('c-reply', className),
        toggle: classNames('c-reply_toggle', 'u-text-bold', {
            ['u-mb-4']: isReplyAccordionOpen
        }),
        content: classNames('u-mb-4'),
        icon: classNames('c-reply_toggle-icon', 'u-w-5', 'u-h-5', 'u-mr-2', 'u-fill-theme-8', {
            'c-reply_toggle-icon--open': isReplyAccordionOpen
        })
    };

    useEffect(() => {

        setShouldRenderCancelButton(true);

    }, []);
    const accordionToggleContent: JSX.Element = <><SVGIcon name="icon-reply" className={generatedClasses.icon} /><span>{reply}</span></>

    return (

        <div ref={wrapperRef} className={generatedClasses.wrapper}>
            <Accordion
                id={generatedIds.replyAccordion}
                isOpen={isReplyAccordionOpen}
                toggleOpenChildren={accordionToggleContent}
                toggleClosedChildren={accordionToggleContent}
                toggleAction={handleToggle}
                toggleClassName={generatedClasses.toggle}
                contentClassName={generatedClasses.content}>
                    <Form
                        formId={formTypes.CREATE_DISCUSSION_COMMENT_REPLY}
                        instanceId={targetId}
                        csrfToken={csrfToken}
                        fields={fields}
                        initialErrors={initialErrors}
                        text={{
                            submitButton: 'Reply',
                            cancelButton: 'Cancel'
                        }}
                        validationFailAction={validationFailAction}
                        cancelAction={handleCancel}
                        submitAction={handleSubmit}
                        className="u-mt-6" />
            </Accordion>
        </div>

    );
}