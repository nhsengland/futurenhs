import React, { useEffect, useRef, useState } from 'react'
import { useRouter } from 'next/router'
import classNames from 'classnames'

import { Props } from './interfaces'

export const Accordion: (props: Props) => JSX.Element = ({
    id,
    children,
    toggleOpenChildren,
    toggleClosedChildren,
    className,
    toggleClassName,
    contentClassName,
    isDisabled = false,
    isOpen = false,
    shouldCloseOnLeave = false,
    shouldCloseOnContentClick = false,
    shouldCloseOnRouteChange = false,
    toggleAction,
}) => {
    const wrapperRef: any = useRef(null)
    const router: any = useRouter()
    const [internalIsOpen, setInternalIsOpen] = useState(isOpen)

    const generatedClasses: any = {
        wrapper: classNames('c-accordion', className, {
            ['c-accordion--open']: internalIsOpen,
            ['c-accordion--disabled']: isDisabled,
        }),
        toggle: classNames('c-accordion_toggle', toggleClassName, {
            ['c-accordion_toggle--open']: internalIsOpen,
        }),
        content: classNames('c-accordion_content', contentClassName, {
            ['c-accordion_content--open']: internalIsOpen,
        }),
    }

    /**
     * On toggle update state
     */
    const onToggle = () => {
        const isOpen: boolean = wrapperRef.current?.hasAttribute('open')

        setInternalIsOpen(isOpen)
        toggleAction?.(id, isOpen)
    }

    /**
     * Toggle the collapsed state of the content area
     */
    const handleToggle = (): void => {
        wrapperRef.current?.toggleAttribute('open')
    }

    /**
     * Handles click event
     */
    const handleClick: any = (event: Event): void => {
        if (isDisabled) {
            event.preventDefault()
        }
    }

    /**
     * Handles content event
     */
    const handleContentClick: any = (event: any): void => {
        if (
            wrapperRef.current &&
            shouldCloseOnRouteChange &&
            event.target.tagName === 'A'
        ) {
            wrapperRef.current?.removeAttribute('open')
        } else if (shouldCloseOnContentClick && wrapperRef.current) {
            wrapperRef.current?.removeAttribute('open')
        }
    }

    /**
     * Handles click or focus outside the component
     */
    const handleLeave: any = (event: Event): void => {
        if (
            wrapperRef.current &&
            !wrapperRef.current.contains(event.target) &&
            shouldCloseOnLeave &&
            !isDisabled
        ) {
            wrapperRef.current?.removeAttribute('open')
        }
    }

    /**
     * Conditionally toggle the accordion on component mount
     * To accomodate server rendering, we must always default to rendering tha accordion expanded
     */
    useEffect(() => {
        document.addEventListener('focusin', handleLeave, false)
        document.addEventListener('click', handleLeave, false)

        return () => {
            document.removeEventListener('focusin', handleLeave, false)
            document.removeEventListener('click', handleLeave, false)
        }
    }, [shouldCloseOnLeave])

    /**
     * Update internal isOpen state when prop changes
     */
    useEffect(() => {
        setInternalIsOpen(isOpen)
    }, [isOpen])

    return (
        <details
            ref={wrapperRef}
            className={generatedClasses.wrapper}
            open={internalIsOpen}
            onToggle={onToggle}
            onClick={handleClick}
        >
            <summary className={generatedClasses.toggle}>
                {internalIsOpen ? toggleOpenChildren : toggleClosedChildren}
            </summary>
            <div
                id={id}
                className={generatedClasses.content}
                onClick={handleContentClick}
            >
                {React.Children.map(children, (child: any) => {
                    const isReactComponent: boolean =
                        typeof child?.type === 'function'
                    const propsToMerge: any = isReactComponent
                        ? {
                              toggleAction: handleToggle,
                          }
                        : {}

                    return React.cloneElement(child, propsToMerge)
                })}
            </div>
        </details>
    )
}
