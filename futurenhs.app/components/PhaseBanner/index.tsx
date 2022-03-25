import React from 'react';
import classNames from 'classnames';

import { RichText } from '@components/RichText';

import { Props } from './interfaces';

export const PhaseBanner: (props: Props) => JSX.Element = ({
    text,
    className,
}) => {

    const { tag, body } = text;

    const generatedClasses: any = {
        wrapper: classNames('c-phase-banner', className),
        tag: classNames('c-phase-banner_tag'),
        content: classNames('c-phase-banner_content')
    };

    return (

        <div className={generatedClasses.wrapper}>
            <p className={generatedClasses.tag}>
                <strong>{tag}</strong>
            </p>
            <RichText className={generatedClasses.content}wrapperElementType="p" bodyHtml={body} />
        </div>

    );

}