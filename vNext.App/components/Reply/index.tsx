import React from 'react';
import classNames from 'classnames';

import { formTypes } from '@constants/forms';
import { Form } from '@components/Form';
import { Accordion } from '@components/Accordion';
import { SVGIcon } from '@components/SVGIcon';
import { selectFormDefaultFields } from '@selectors/forms';
import forms from '@formConfigs/index';

import { Props } from './interfaces';

export const Reply: (props: Props) => JSX.Element = ({
    targetId,
    text,
    className
}) => {

    const fields = selectFormDefaultFields(forms, formTypes.CREATE_DISCUSSION_COMMENT_REPLY);
    const { reply } = text;

    console.log(fields);

    const generatedIds: any = {
        accordion: `${targetId}-reply-accordion`
    };

    const generatedClasses: any = {
        wrapper: classNames('c-reply', className),
        toggle: classNames('c-reply_toggle', 'u-text-bold'),
        icon: classNames('u-w-5', 'u-h-5', 'u-mr-2', 'u-fill-theme-8')
    };

    return (

        <div className={generatedClasses.wrapper}>
            <Accordion 
                id={generatedIds.replyAccordion}
                toggleChildren={<><SVGIcon name="icon-reply" className={generatedClasses.icon} /><span>{reply}</span></>} 
                toggleClassName={generatedClasses.toggle}>
                    <Form 
                        csrfToken="" 
                        fields={fields} 
                        text={{
                            submitButton: 'Reply'
                        }} 
                        submitAction={() => {}}
                        className="u-mt-6" />
            </Accordion>
        </div>

    );
}