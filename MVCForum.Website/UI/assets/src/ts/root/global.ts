/**
 * Modern browser feature checks
 * These will need updating as browser support and site features evolve
 */
const isFetchSupported = (): boolean => Boolean(window.fetch);
const isPromiseSupported = (): boolean => Boolean(window.Promise);
const isSymbolSupported = (): boolean => Boolean(window.Symbol);
const isDetailsElementSupported = (): boolean => 'open' in document.createElement('details');

/**
 * Check whether polyfills are required for current browser
 * This is a high level check for modern features which favours performance for the majority of users on modern browsers
 * which don't require any polyfills at all - and the check can be performed with no additioal http requests.
 * Slightly older browsers may need to load the polyfills file but only require some of the contents, but this is the trade-off for being
 * able to keep the feature check very simple
 */
const shouldLoadPolyFills = !isFetchSupported() || !isPromiseSupported() || !isSymbolSupported() || !isDetailsElementSupported();

/**
 * Load polyfills
 */
const loadPolyFills = (src: string, done): void => {

    const polyFillsScript: HTMLScriptElement = document.createElement('script');

    document.body.appendChild(polyFillsScript);

    polyFillsScript.onload = () => {

        done();

    };

    polyFillsScript.onerror = () => {

        done(new Error('Failed to load polyfills'));

    };

    polyFillsScript.src = src;

}

/**
 * temporary hardcoded js config
 */
window.jsConfig = {
    adminClassNameEndPointMap: {
        'dashboardlatestusers': 'LatestUsers',
        'dashboardlowestpointusers': 'LowestPointUsers',
        'dashboardlowestpointposts': 'LowestPointPosts',
        'dashboardtodaystopics': 'TodaysTopics',
        'dashboardhighestviewedtopics': 'HighestViewedTopics',
    },
    tagsInputAdditionalConfig: {
        minChars: 2,
        maxChars: 25,
        removeWithBackspace: true,
        autocomplete_url: '/tag/autocompletetags',
    }
}


/**
 * Init JS
 */
const jsInit = (): void => {

    import('@modules/ui/index').then(({ uiComponentsInit }) => {

        uiComponentsInit(window.jsConfig);

    });

    import('@modules/utilities/index').then(({ utilitiesInit }) => {

        utilitiesInit();

    });

}

/**
 * If polyfills are required, fetch these first before initialising JS,
 * else init JS directly
 */
if (shouldLoadPolyFills) {

    loadPolyFills('/UI/assets/dist/js/polyfills.min.js', jsInit);

} else {

    jsInit();

}
