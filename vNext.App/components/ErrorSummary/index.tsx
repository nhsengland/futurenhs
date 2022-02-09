import React from 'react';
import classNames from 'classnames';

import { scrollToComponentAndSetFocus } from '@helpers/dom';
import { RichText } from '@components/RichText';

import { Props } from './interfaces';

/**
 * Renders a form submission validation error summary
 */
export const ErrorSummary = React.forwardRef(({ 
    errors = {},
    relatedNames = [],
    text,
    className
}: Props, ref) => {

    const { body } = text ?? {};

    const generatedClasses: any = {
        wrapper: classNames('c-error-summary', className),
        list: classNames('c-error-summary_list', 'u-m-0', 'u-p-0', 'u-list-none'),
        listItem: classNames('c-error-summary_list-item'),
        link: classNames('c-error-summary_link'),
        item: classNames('c-error-summary_item')
    };

    const hasErrors: boolean = errors && Object.keys(errors).length > 0;

    return (

        <div
            ref={ref as any} 
            aria-live="assertive" 
            aria-atomic="true" 
            aria-relevant="additions" 
            tabIndex={-1}>
                {hasErrors &&
                    <div className={generatedClasses.wrapper}>
                        {body &&
                            <RichText 
                                bodyHtml={body} 
                                wrapperElementType="p"
                                className="u-text-bold u-text-lead" />
                        }
                        <ul className={generatedClasses.list}>
                            {Object.keys(errors).map((key, index: number) => {

                                const handleClick = (event: any) => {

                                    event.preventDefault();

                                    const rteInstance: any = (window as any).tinymce?.get(key);

                                    if(rteInstance){

                                        const rteElement: HTMLElement = rteInstance.getContentAreaContainer();

                                        scrollToComponentAndSetFocus(rteElement, false, 60);
                                        rteInstance.focus();

                                    } else {

                                        const element: HTMLElement = document.getElementById(key);

                                        scrollToComponentAndSetFocus(element, false, 60);

                                    }

                                }

                                return (

                                    <li key={index} className={generatedClasses.listItem}>
                                        {relatedNames.includes(key)
                                        
                                            ?   <a href={`#${key}`} className={generatedClasses.link} onClick={handleClick}>{errors[key]}</a>
                                            
                                            :   <span className={generatedClasses.item}>{errors[key]}</span> 
                                        
                                        }
                                    </li>

                                )
                                
                            })}
                        </ul>
                    </div>
                }
        </div>

    )

});