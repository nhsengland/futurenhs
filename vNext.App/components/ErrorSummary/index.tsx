import React from 'react';
import classNames from 'classnames';

import { RichText } from '@components/RichText';

import { Props } from './interfaces';

export const ErrorSummary = React.forwardRef(({ 
    errors = [],
    relatedNames = [],
    text,
    className
}: Props, ref) => {

    const { body } = text ?? {};

    const generatedClasses: any = {
        wrapper: classNames('c-error-summary', className),
        list: classNames('c-error-summary_list', 'u-m-0', 'u-p-0', 'u-list-none'),
        listItem: classNames('c-error-summary_list-item'),
        link: classNames('c-error-summary_link')
    };

    const hasErrors: boolean = errors?.length > 0;

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
                            {errors.map((error, index: number) => {

                                const key = Object.keys(error)[0];

                                return (

                                    <li key={index} className={generatedClasses.listItem}>
                                        {relatedNames.includes(key)
                                        
                                            ?   <a href={`#${key}`} className={generatedClasses.link}>{error[key]}</a>
                                            
                                            :   <span className={generatedClasses.link}>{error[key]}</span> 
                                        
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