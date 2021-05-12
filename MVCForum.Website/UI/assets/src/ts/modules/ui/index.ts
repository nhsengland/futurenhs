import { ping } from '@utilities/routing';
import { TagsInputAPIInterface } from '@modules/ui/components/tagsInput';

export const uiComponentsInit = (config: {
    adminClassNameEndPointMap: {
        [key: string]: string
    },
    tagsInputAdditionalConfig: TagsInputAPIInterface
}) => {

    let toast: any = undefined;

    /**
     * Ping the back end to let it know the user is still online
     */
    ping(window.app_base + "Members/LastActiveCheck");


    /**
     * Init the main site nav
     */
    const siteHeaderNavElement: HTMLElement = document.querySelector('.js-site-header-nav');

    if(siteHeaderNavElement){
 
        import('@modules/ui/components/siteHeaderNav').then(({ SiteHeaderNav }) => {
 
            new SiteHeaderNav({
                wrapperSelector: siteHeaderNavElement
            });
 
        });
 
    }

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
     * Init email subscribe /unsubscribe
     */
    import('@modules/ui/components/emailSubscription').then(({ EmailSubscription }) => {

        new EmailSubscription({
            wrapperSelector: undefined,
        }, {
            components: {
                toast: toast
            }
        });

    });

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

    /**
    * Init admin dashboard
    */
    const adminDashboardElements: Array<Element> = Array.from(document.getElementsByClassName('admindashboard'));

    if (adminDashboardElements?.length > 0) {

        import('@modules/ui/components/adminDashboard').then(({ AdminDashboard }) => {

            new AdminDashboard({
                wrapperSelector: undefined,
                fetchTargets: config.adminClassNameEndPointMap
            }).bindDataToHtmlElements();

        });

    }

    /**
    * Init tags input
    */
    const tagsTextareaElements: Array<Element> = Array.from(document.getElementsByClassName('tagstextarea'));

    if (tagsTextareaElements?.length > 0) {

        import('@modules/ui/components/tagsInput').then(({ TagsInput }) => {

            tagsTextareaElements.forEach((tagTextarea: HTMLFormElement) => {

                new TagsInput({
                    wrapperSelector: tagTextarea,
                    tagsInputAdditionalConfig: config.tagsInputAdditionalConfig
                });

            });

        });

    }



}