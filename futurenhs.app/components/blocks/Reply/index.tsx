import { useState, useRef, useEffect } from 'react'
import classNames from 'classnames'

import { formTypes } from '@constants/forms'
import { Form } from '@components/old_forms/Form'
import { Accordion } from '@components/generic/Accordion'
import { SVGIcon } from '@components/generic/SVGIcon'
import forms from '@config/form-configs/index'
import { FormConfig, FormErrors } from '@appTypes/form'

import { Props } from './interfaces'
import { useFormConfig } from '@helpers/hooks/useForm'

export const Reply: (props: Props) => JSX.Element = ({
    targetId,
    csrfToken,
    text,
    validationFailAction,
    submitAction,
    className,
}) => {
    const wrapperRef = useRef()

    const formConfig: FormConfig = useFormConfig(
        formTypes.CREATE_DISCUSSION_COMMENT_REPLY,
        forms[formTypes.CREATE_DISCUSSION_COMMENT_REPLY]
    )

    const [isReplyAccordionOpen, setIsReplyAccordionOpen] = useState(false)
    const [shouldRenderCancelButton, setShouldRenderCancelButton] =
        useState(false)

    const { reply } = text

    const handleToggle = (_, isOpen) => {
        setIsReplyAccordionOpen(isOpen)
    }

    const handleCancel = shouldRenderCancelButton
        ? (): void => {
              const summaryElement: HTMLElement = (
                  wrapperRef.current as any
              ).getElementsByTagName('SUMMARY')?.[0] as HTMLElement

              setIsReplyAccordionOpen(false)
              summaryElement?.focus()
          }
        : null

    const handleSubmit = (formData: FormData): Promise<FormErrors> => {
        setIsReplyAccordionOpen(false)

        return submitAction(formData)
    }

    const generatedIds: any = {
        accordion: `${targetId}-reply-accordion`,
    }

    const generatedClasses: any = {
        wrapper: classNames('c-reply', className),
        toggle: classNames('c-reply_toggle', 'u-text-bold', {
            ['u-mb-4']: isReplyAccordionOpen,
        }),
        content: classNames('u-mb-4'),
        icon: classNames(
            'c-reply_toggle-icon',
            'u-w-5',
            'u-h-5',
            'u-mr-2',
            'u-fill-theme-8',
            {
                'c-reply_toggle-icon--open': isReplyAccordionOpen,
            }
        ),
    }

    const accordionToggleContent: JSX.Element = (
        <>
            <SVGIcon name="icon-reply" className={generatedClasses.icon} />
            <span>{reply}</span>
        </>
    )

    useEffect(() => {
        setShouldRenderCancelButton(true)
    }, [])

    return (
        <div ref={wrapperRef} className={generatedClasses.wrapper}>
            <Accordion
                id={generatedIds.replyAccordion}
                isOpen={isReplyAccordionOpen}
                toggleOpenChildren={accordionToggleContent}
                toggleClosedChildren={accordionToggleContent}
                toggleAction={handleToggle}
                toggleClassName={generatedClasses.toggle}
                contentClassName={generatedClasses.content}
            >
                <Form
                    instanceId={targetId}
                    csrfToken={csrfToken}
                    formConfig={formConfig}
                    text={{
                        submitButton: 'Reply',
                        cancelButton: 'Discard Reply',
                    }}
                    shouldClearOnSubmitSuccess={true}
                    validationFailAction={validationFailAction}
                    cancelAction={handleCancel}
                    submitAction={handleSubmit}
                    className="u-mt-6"
                />
            </Accordion>
        </div>
    )
}
