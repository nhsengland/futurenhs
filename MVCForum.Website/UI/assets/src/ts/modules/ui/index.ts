import { pushgtmDataLayerEvent } from '@utilities/gtmDataLayer';
import { ping } from '@utilities/routing'

export const uiComponentsInit = (config: {

}) => {

    /**
     * Ping the back end to let it know the user is still online
     */
    ping(window.app_base + "Members/LastActiveCheck");

    /**
     * Init mobile nav
     */
    import('@modules/ui/components/mobileNav').then(({ MobileNav }) => {

        new MobileNav({});

    });

    /**
     * Init email subscribe /unsubscribe
     */
     import('@modules/ui/components/emailSubscription').then(({ EmailSubscription }) => {

        new EmailSubscription({});

    });

    /**
     * Init upload buttons
     */
     const uploadButtons: Array<HTMLElement> = Array.from(document.querySelectorAll('.btn-file'));

     if (uploadButtons?.length > 0) {
 
        import('@modules/ui/components/uploadButton').then(({ UploadButton }) => {
 
            uploadButtons.forEach((uploadButtonElement: HTMLElement) => new UploadButton({
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

            detailsElements.forEach((detailsElement: HTMLDetailsElement) => new Details(detailsElement));

        });

    }

    /**
     * Init ajax forms
     */
    const ajaxForms: Array<HTMLFormElement> = Array.from(document.querySelectorAll('.ajaxform form'));

    if (ajaxForms?.length > 0) {

        import('@modules/ui/components/ajaxForm').then(({ AjaxForm }) => {

            ajaxForms.forEach((ajaxForm: HTMLFormElement) => new AjaxForm({
                wrapperSelector: ajaxForm,
                successCallBack: (result: any) => {

                    if (result.Success) {

                        window.closeSlideOutPanel();
                        window.ShowUserMessage(result.ReturnMessage);

                    } else {

                        window.ShowUserMessage(result.ReturnMessage);

                    }

                },
                errorCallBack: (xhr, ajaxOptions, thrownError) => {

                    window.ShowUserMessage("Error: " + xhr.status + " " + thrownError);

                }
            }));

        });

    }

}