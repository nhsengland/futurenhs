import { useRef, useEffect } from 'react';
import classNames from 'classnames';
import isFocusable from 'ally.js/is/focusable';

import { Props } from './interfaces';

/**
 * Generic handling of dynamically updated list content
 */
export const DynamicListContainer: (props: Props) => JSX.Element = ({
    containerElementType,
    shouldFocusLatest,
    children,
    className
}) => {

    const containerRef = useRef();
    const childElementCount = useRef(0);

    const containerElementsAllowList: Array<string> = ['div', 'ul', 'ol', 'tbody'];
    const Container = containerElementType;

    if(!containerElementsAllowList.includes(containerElementType)){

        throw new Error('Invalid containerElementType');

    }

    const generatedClasses: any = {
        wrapper: classNames(className)
    }

    useEffect(() => {

        /**
         * Generic handling of dynamically updated list content
         */
        const updatedChildElementCount: number = (containerRef.current as HTMLElement)?.children?.length ?? 0;
        const hasMoreChildren: boolean = updatedChildElementCount > childElementCount.current && childElementCount.current > 0;

        /**
         * If focus latest mode enabled and more children were just added
         */
        if(hasMoreChildren && shouldFocusLatest){

            /**
             * Get the index of the first of the newest children - which is equal to the length of the previous list of children
             */
            const newestChildElement: HTMLElement = (containerRef.current as any)?.children[childElementCount.current];

            /**
             * If the child is not natively focusable, make it temporarily so
             */
            if(!isFocusable(newestChildElement)){

                newestChildElement?.setAttribute('tabIndex', '-1');
                newestChildElement?.addEventListener('blur', (event) => { 
                    newestChildElement.removeAttribute('tabIndex'), { 
                        once: true
                    }
                });

            }

            /**
             * Focus the child
             */
            newestChildElement?.focus();

        }

        childElementCount.current = updatedChildElementCount;

    }, [children]);

    return (

        <>
            <Container ref={containerRef} className={generatedClasses.wrapper}>
                {children}
            </Container>
        </>

    );

}