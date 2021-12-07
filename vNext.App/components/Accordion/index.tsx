import React, { useEffect, useRef } from 'react';
import classNames from 'classnames';

import { Props } from './interfaces';

export const Accordion: (props: Props) => JSX.Element = ({
    id,
    children,
    toggleChildren,
    className,
    toggleClassName,
    contentClassName,
    isDisabled = false,
    isOpen = false,
    shouldCloseOnLeave = false,
    toggleAction
}) => {

    const wrapperRef: any = useRef(null);

    const generatedClasses: any = {
        wrapper: classNames('c-accordion', className, {
            ['c-accordion--open']: isOpen,
            ['c-accordion--disabled']: isDisabled
        }),
        toggle: classNames('c-accordion_toggle', toggleClassName, {
            ['c-accordion_toggle--open']: isOpen
        }),
        content: classNames('c-accordion_content', contentClassName, {
            ['c-accordion_content--open']: isOpen
        })
    };

    /**
     * On toggle update state
     */
    const onToggle = () => {

        toggleAction?.(id, wrapperRef.current?.hasAttribute('open'));

    }
    
    /**
     * Toggle the collapsed state of the content area
     */
    const handleToggle = (): void => {
        
        wrapperRef.current?.toggleAttribute('open');

    }

    /**
     * Handles click event
     */
    const handleClick: any = (event: Event): void => {

        if (isDisabled) {

            event.preventDefault();

        }

    }

    /**
     * Handles click or focus outside the component
     */
    const handleLeave: any = (event: Event): void => {

        if (wrapperRef.current && !wrapperRef.current.contains(event.target) && shouldCloseOnLeave && !isDisabled) {

            wrapperRef.current?.removeAttribute('open');

        }

    }

    /**
     * Conditionally toggle the accordion on component mount
     * To accomodate server rendering, we must always default to rendering tha accordion expanded
     */
    useEffect(() => {

        document.addEventListener('focusin', handleLeave, false);
        document.addEventListener('click', handleLeave, false);

        return () => {

            document.removeEventListener('focusin', handleLeave, false);
            document.removeEventListener('click', handleLeave, false);
        
        };


    });

    return (

        <details 
            ref={wrapperRef} 
            className={generatedClasses.wrapper}
            open={isOpen}
            onToggle={onToggle}
            onClick={handleClick}>
                <summary className={generatedClasses.toggle}>
                    {toggleChildren}
                </summary>
                <div id={id} className={generatedClasses.content}>
                    {React.Children.map(children, (child: any) => {

                        const isReactComponent: boolean = typeof child?.type === 'function';
                        const propsToMerge: any = isReactComponent ? {
                            toggleAction: handleToggle
                        } : {};

                        return React.cloneElement(child, propsToMerge);

                    })}
                </div>
        </details>

    );
}