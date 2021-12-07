import React, { useEffect } from 'react';
import classNames from 'classnames';

import { RichText } from '@components/RichText';

import { Props } from './interfaces';

export const ErrorSummary = React.forwardRef(({ 
    errors = {},
    relatedNames = [],
    content,
    className
}: Props, ref) => {

    const { bodyHtml } = content;

    const generatedClasses: any = {
        wrapper: classNames('c-info-message', 'c-info-message--error', className),
        list: classNames('u-m-0', 'u-p-0', 'u-list-none')
    };

    const hasErrors: boolean = Boolean(Object.keys(errors).length > 0);

    return (

        <div
            ref={ref as any} 
            aria-live="assertive" 
            aria-atomic="true" 
            aria-relevant="additions" 
            tabIndex={-1}>
                {hasErrors &&
                    <div className={generatedClasses.wrapper}>
                        <RichText bodyHtml={bodyHtml} wrapperElementType="p" />
                        <ul className={generatedClasses.list}>
                            {Object.keys(errors).map((key: string, index: number) => {

                                return (

                                    <li key={index}>
                                        {relatedNames.includes(key)
                                        
                                            ?   <a href={`#${key}`}>{errors[key]}</a>
                                            
                                            :   <span>{errors[key]}</span> 
                                        
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