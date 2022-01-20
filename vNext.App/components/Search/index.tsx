import { useState } from 'react';
import classNames from 'classnames';

import { SVGIcon } from '@components/SVGIcon';

import { Props } from './interfaces';

export const Search: (props: Props) => JSX.Element = ({
    value,
    method,
    action,
    id,
    content,
    className
}) => {

    const [dynamicValue, setDynamicValue] = useState(value);
    
    const { labelText, placeholderText } = content ?? {};

    const handleSearchUpdate = (event) => setDynamicValue(event.target.value);

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
                    value={dynamicValue} 
                    placeholder={placeholderText} 
                    type="search"
                    onChange={handleSearchUpdate} />
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