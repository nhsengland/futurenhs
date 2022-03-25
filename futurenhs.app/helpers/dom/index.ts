/**
 * Gets a consistently formatted commentId
 */
export const getFormattedCommentId: Function = (commentId: string) => {

    if(!commentId){

        throw new Error('Missing commentId')

    }

    return `comment-${commentId}`;

}

/**
 * Lock scroll on page body
 */
export const lockBodyScroll: Function = (shouldLock: boolean) => {

    if(shouldLock){

        document.body.classList.add('u-overflow-hidden');

    } else {

        document.body.classList.remove('u-overflow-hidden');

    }


}

/**
 * Gets positional data required to scroll a component into view
 */
export const getScrollToPositionForComponent = (element: HTMLElement, customOffsetTop: number = 0): any => {

	if(window && element){

		const boundingClientRect: ClientRect = element.getBoundingClientRect();
		
		const { right, top } = boundingClientRect;

		const offsetTop: number = window.pageYOffset || document.documentElement.scrollTop;
		const offsetLeft: number = window.pageXOffset || document.documentElement.scrollLeft;

		return {
			x: right + offsetLeft,
			y: top + offsetTop - customOffsetTop
		};

	}

	return null;

};

/**
 * Gets the first focusable child in an element
 */
export const getFirstFocusableElementInComponent = (element: HTMLElement, shouldFocusFirstChild?: boolean): any => {

	if(element){

		if(!shouldFocusFirstChild){

			return element;

		}

        const focusableChildElements: NodeListOf<HTMLElement> = element.querySelectorAll('button, [href], input, select, textarea');
		
		return focusableChildElements.length ? focusableChildElements[0] : null;

	}

	return null;

};

/**
 * Focus an element on scroll complete
 */
export const focusElementOnScrollComplete = (element: HTMLElement, customOffsetTop: number): void => {

	const isElementInDesiredPosition = (): boolean => {

		const offsetTop: number = (window && window.pageYOffset) || (document && document.documentElement.scrollTop) || 0;

		return offsetTop -1 < customOffsetTop;

	};

	if(window && element){

		if (isElementInDesiredPosition()) {

			window.setTimeout(() => element.focus(), 0);

		} else {

			window.onscroll = () => {

				if (isElementInDesiredPosition()) {
		
                    window.setTimeout(() => element.focus(), 0);
					window.onscroll = null;
		
				}
		
			};

		}

	}

};

/**
 * Scroll nicely
 */
export const prettyScroll = (left: number, top: number, shouldSmoothScroll: boolean = true): void => {

    const isSmoothScrollSupported: boolean = 'scrollBehavior' in document.documentElement.style;

    if(typeof left === 'number' && typeof top === 'number'){

	    if(shouldSmoothScroll && isSmoothScrollSupported){

	        window.scrollTo({
	            behavior: 'smooth',
	            left: left,
	            top: top
	        });

	    } else {

	        window.scrollTo(left, top);

	    }

	}

};

/**
 * Scroll to an element and set focus on either the element or its first focusable child
 */
 export const scrollToComponentAndSetFocus = (element: HTMLElement, shouldFocusFirstChild: boolean, customOffsetTop: number): void => {

	if(element){

		/**
		 * Get DOM positional data for scrolling and focus management
		 */
		const scrollPositionData: any = getScrollToPositionForComponent(element, customOffsetTop);
        const elementToFocus: HTMLElement = getFirstFocusableElementInComponent(element, shouldFocusFirstChild);
        
		/**
		 * Find the underlying DOM node for the component and scroll to it
		 */
		if (scrollPositionData && elementToFocus) {

			const { x, y } = scrollPositionData;
	
			/**
			 * If component has focusable child elements, focus the first one when in correct position
			 * in viewport on auto scroll
			 */
			focusElementOnScrollComplete(elementToFocus, y);

			/**
			 * Trigger auto scroll to correct position on page
			 */
			prettyScroll(x, y);

		}

	}

};