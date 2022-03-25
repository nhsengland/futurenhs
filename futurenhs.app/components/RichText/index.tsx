import React from 'react';
import htmlTags from 'html-tags';
import classNames from 'classnames';

import { Props } from './interfaces';

export const RichText: (props: Props) => JSX.Element = ({ 
    id,
    bodyHtml,
    className,
    ariaLabelledBy,
    ariaDescribedBy,
    wrapperElementType,
    shouldPadRight = false,
    shouldPadleft = false,
    stripHtmlPattern = null
}) => {

    /**
     * Renders HTML
     */
    const createMarkup = (bodyHtml: string): any => ({ __html: bodyHtml });
    
    const basic = /\s?<!doctype html>|(<html\b[^>]*>|<body\b[^>]*>|<x-[^>]+>)+/i;
    const full = new RegExp(htmlTags.map(tag => `<${tag}\\b[^>]*>`).join('|'), 'i');

    const isHtml = (string: string): boolean => {

        string = string?.trim().slice(0, 1000) ?? '';

        return basic.test(string) || full.test(string);

    }

    const shouldRenderHTML: boolean = isHtml(bodyHtml) ;
    const tagsWhiteList: Array<string> = ['div', 'span', 'p', 'h1', 'h2', 'h3', 'h4', 'h5', 'h6'];
    const padLeft = shouldPadleft ? " " : "";
    const padRight = shouldPadRight ? " " : "";

    bodyHtml =  stripHtmlPattern && bodyHtml ? bodyHtml.replace(stripHtmlPattern,'') : bodyHtml;

    /**
     * Get the wrapper element type to use
     * Use the type requested as a prop, or default to 'div' when bodyHtml contains HTML, or no wrapper when bodyHtml is plain text
     */
    const WrapperElement: any = wrapperElementType && tagsWhiteList.includes(wrapperElementType)

        ?   wrapperElementType
        :   shouldRenderHTML

                ?   'div'
                :   undefined;

    const generatedClasses: any = {
        wrapper: classNames('c-rich-text', className)
    };

    if(shouldRenderHTML){

        return (

            <WrapperElement
                id={id}
                className={generatedClasses.wrapper}
                aria-labelledby={ariaLabelledBy}
                aria-describedby={ariaDescribedBy}
                dangerouslySetInnerHTML={createMarkup(bodyHtml)}>
            </WrapperElement>

        );

    } else if(Boolean(WrapperElement)){

        return (

            <WrapperElement
                id={id}
                className={generatedClasses.wrapper}
                aria-labelledby={ariaLabelledBy}
                aria-describedby={ariaDescribedBy}>
                    {padLeft}{bodyHtml}{padRight}
            </WrapperElement>

        );

    }

    return (

        <>{bodyHtml}</>

    );

}

