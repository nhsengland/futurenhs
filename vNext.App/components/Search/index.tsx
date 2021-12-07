import React from 'react';
import classNames from 'classnames';

import { SVGIcon } from '@components/SVGIcon';

import { Props } from './interfaces';

export const Search: (props: Props) => JSX.Element = ({
    method,
    action,
    id,
    content,
    className
}) => {

    const { labelText, placeholderText } = content ?? {};

    const generatedClasses: any = {
        wrapper: classNames(`c-site-header-nav_search-wrapper`, className),
        label: classNames(`u-sr-only`),
        input: classNames(`c-site-header-nav_search-input`),
        button: classNames(`c-site-header-nav_search-button`),
        buttonIcon: classNames(`c-site-header-nav_search-button-icon`)
    };

    return (

        <form method={method} action={action} className={generatedClasses.wrapper}>
            <div className="c-site-header-nav_search-item">
                <label className={generatedClasses.label} htmlFor={id}>{labelText}</label>
                <input 
                    aria-label="Search" 
                    className={generatedClasses.input} 
                    id={id} 
                    name={id} 
                    placeholder={placeholderText} 
                    type="search" />
                <button 
                    type="submit" 
                    aria-label="Search button" 
                    className={generatedClasses.button}>
                        <SVGIcon name="icon-search" className={generatedClasses.buttonIcon} />
                </button>
            </div>
        </form>

    );

}