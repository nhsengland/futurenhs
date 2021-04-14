/**
 * Gets whether prefers reduced motion flag is enabled
 */
export const hasPrefersReducedMotionEnabled: Function = (): boolean => {

    const query: MediaQueryList = window?.matchMedia('(prefers-reduced-motion: reduce)');

    return Boolean(query?.matches);

}

/**
 * Gets whether browser supports smooth scroll
 */
export const isSmoothScrollSupported: Function = (): boolean => {

    return 'scrollBehavior' in document.documentElement.style;

};

/**
 * Scrolls to a page position, using animation when appropriate
 */
export const fancyScrollTo: Function = (offSet: number): void => {

    if(isSmoothScrollSupported() && !hasPrefersReducedMotionEnabled()){

        window.scrollTo({
            top: offSet,
            left: 0,
            behavior: 'smooth'
        });

    } else {

        window.scrollTo(0, offSet);

    }

}
