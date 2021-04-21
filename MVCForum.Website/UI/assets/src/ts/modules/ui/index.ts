import { pushgtmDataLayerEvent } from '@utilities/gtmDataLayer';
import { ping } from '@utilities/routing';

export const uiComponentsInit = (config: {

}) => {

    let toast: any = undefined;

    /**
     * Ping the back end to let it know the user is still online
     */
    ping(window.app_base + "Members/LastActiveCheck");

    /**
     * Init mobile nav
     */
    import('@modules/ui/components/mobileNav').then(({ MobileNav }) => {

        new MobileNav({
            wrapperSelector: undefined,
            triggerButtonSelector: document.querySelector('.showmobilenavbar'),
            contentSelector: document.querySelector('.mobilenavbar-inner ul.nav')
        });

    });

    /**
     * Init email subscribe /unsubscribe
     */
     import('@modules/ui/components/emailSubscription').then(({ EmailSubscription }) => {

        new EmailSubscription({
            wrapperSelector: undefined
        });

    });
    
    /**
     * Init toast
     */
     const toastElement: HTMLElement = document.getElementById('js-toast');

     if (toastElement) {
 
        import('@modules/ui/components/toast').then(({ Toast }) => {

            const message: string = toastElement.dataset?.message;
            const timeOut: number = toastElement.dataset?.timeout ? parseInt(toastElement.dataset?.timeout, 10) : undefined;
 
            toast = new Toast({
                wrapperSelector: toastElement,
                timeOutMillis: timeOut,
                messageText: message
            });
 
        });
 
    }

    /**
     * Init language switchers
     */
     const languageSwitcherElements: Array<HTMLElement> = Array.from(document.querySelectorAll('.js-language-selector'));

     if (languageSwitcherElements.length > 0) {
 
        import('@modules/ui/components/languageSwitcher').then(({ LanguageSwitcher }) => {

            languageSwitcherElements.forEach((languageSwitcherElement: HTMLSelectElement) => {
                
                const languageSwitcher = new LanguageSwitcher({
                    wrapperSelector: languageSwitcherElement
                });

                languageSwitcher.on('success', () => window.location.reload());
                languageSwitcher.on('error', (errorText: string) => toast?.show(errorText));
        
            });
 
        });
 
    }

    /**
     * Init upload buttons
     */
     const uploadButtonElements: Array<HTMLElement> = Array.from(document.querySelectorAll('.btn-file'));

     if (uploadButtonElements?.length > 0) {
 
        import('@modules/ui/components/uploadButton').then(({ UploadButton }) => {
 
            uploadButtonElements.forEach((uploadButtonElement: HTMLElement) => new UploadButton({
                wrapperSelector: uploadButtonElement
            }));
 
        });
 
    }
    
    /**
     * Init responsive tables
     */
    const responsiveTables: Array<HTMLTableElement> = Array.from(document.querySelectorAll('.table-adaptive'));

     if (responsiveTables?.length > 0) {
 
        import('@modules/ui/components/responsiveTable').then(({ ResponsiveTable }) => {
 
            responsiveTables.forEach((tableElement: HTMLTableElement) => new ResponsiveTable({
                wrapperSelector: tableElement
            }));
 
        });
 
    }
    
    /**
     * Init details accordions
     */
    const detailsElements: Array<HTMLDetailsElement> = Array.from(document.getElementsByTagName('details'));

    if (detailsElements?.length > 0) {

        import('@modules/ui/components/details').then(({ Details }) => {

            detailsElements.forEach((detailsElement: HTMLDetailsElement) => new Details({
                wrapperSelector: detailsElement
            }));

        });

    }

    /**
     * Init ajax forms
     */
    const ajaxForms: Array<HTMLFormElement> = Array.from(document.querySelectorAll('.ajaxform form'));

    if (ajaxForms?.length > 0) {

        import('@modules/ui/components/ajaxForm').then(({ AjaxForm }) => {

            ajaxForms.forEach((ajaxForm: HTMLFormElement) => {
                
                const form = new AjaxForm({
                    wrapperSelector: ajaxForm
                });

                form.on('success', (result) => {

                    const { Success, ReturnMessage } = result;

                    if (Success) {

                        window.closeSlideOutPanel();
                        toast?.show(ReturnMessage);

                    } else {

                        toast?.show(ReturnMessage);

                    }

                });

                form.on('error', (xhr, ajaxOptions, thrownError) => {

                    const { status } = xhr;

                    toast?.show(`Error: ${status} ${thrownError}`);

                });
            
            });

        });

    }

}