import { useRef, useEffect } from 'react';
import classNames from 'classnames';

import { Props } from './interfaces';

/**
 * Generic handling of dynamically updated list content
 */
export const DynamicListContainer: (props: Props) => JSX.Element = ({
    containerElementType,
    shouldEnableLoadMore,
    children,
    className
}) => {

    const containerRef = useRef();
    const childElementCount = useRef(0);

    const containerElementsAllowList: Array<string> = ['div', 'ul', 'ol', 'tbody'];
    const Container = containerElementType;

    if (!containerElementsAllowList.includes(containerElementType)) {

        throw new Error('Invalid containerElementType');

    }

    const generatedClasses: any = {
        wrapper: classNames(className)
    }

    const renderNewItemsAnnouncement = ((newItemsCount: number, newestChildElement: HTMLElement) => {

        /**
         * Determine element type for the announcement from the container element type
         */
        const childElementTypes: Record<string, string> = {
            div: 'div',
            ul: 'li',
            ol: 'li',
            tbody: 'tr'
        };
        const childElementType: string = childElementTypes[containerElementType];

        /**
         * Creates announcement element and removes on blur
         */
        const newItemsAnnouncement: HTMLElement = document.createElement(childElementType);
        newItemsAnnouncement.textContent = `${newItemsCount} new items`;

        newItemsAnnouncement.setAttribute('tabIndex', '-1');
        newItemsAnnouncement.classList.add('u-sr-only')
        newItemsAnnouncement.addEventListener('blur', (event) => {
            newItemsAnnouncement?.remove();
        });


        /**
         * Adds announcement element before the newest item in the list and sets focus
         */
        newestChildElement.before(newItemsAnnouncement);
        newItemsAnnouncement.focus();

    });

    useEffect(() => {

        /**
         * Generic handling of dynamically updated list content
         */
        const updatedChildElementCount: number = (containerRef.current as HTMLElement)?.children?.length ?? 0;
        const hasMoreChildren: boolean = updatedChildElementCount > childElementCount.current && childElementCount.current > 0;
        const newItemsCount: number = updatedChildElementCount - childElementCount.current;

        /**
         * If load more mode enabled and more children were just added
         */
        if (hasMoreChildren && shouldEnableLoadMore) {


            /**
             * Get the index of the first of the newest children - which is equal to the length of the previous list of children
             */
            const newestChildElement: HTMLElement = (containerRef.current as any)?.children[childElementCount.current];

            renderNewItemsAnnouncement(newItemsCount, newestChildElement);

        }

        childElementCount.current = updatedChildElementCount;

    }, [children.length]);

    if (shouldEnableLoadMore) {

        return (
            <Container ref={containerRef} className={generatedClasses.wrapper}>
                {children}
            </Container>
        )

    }

    return (

        <Container ref={containerRef} className={generatedClasses.wrapper} aria-live="polite" aria-atomic="true" aria-relevant="additions">
            {children}
        </Container>

    )

}