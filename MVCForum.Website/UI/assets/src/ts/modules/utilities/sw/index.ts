/**
 * Registers a service worker
 */
export const registerServiceWorker: Function = (): void => {

    if ('serviceWorker' in navigator) {
    
        navigator.serviceWorker.register('./sw.js', { 
            scope: './' 
        })
        .then(() => {

            /**
             * TODO
             */

        })
        .catch((error) => {

            console.log('Service worker registration failed with ' + error);

        });
    
    }

}