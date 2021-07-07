import { ping } from '@utilities/routing';
import * as fetchHelpers from '@utilities/fetch';
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

    if (siteHeaderNavElement) {

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

    /**
    * Init topic post features
    */
    const topicForumContainers: Array<Element> = Array.from(document.getElementsByClassName('topicshow'));

    if (topicForumContainers?.length > 0) {

        import('@modules/ui/components/topicPost').then(({ TopicPost }) => {

            topicForumContainers.forEach((topicForumContainer: HTMLElement) => {

                const fancyBoxTargetTypes: Array<string> = ['gif', 'jpg', 'png', 'bmp', 'jpeg'];
                const pollId: string = (<HTMLInputElement>document.getElementById('Poll_Id'))?.value;

                new TopicPost({
                    pollId: pollId,
                    fancyBoxTargetTypes: fancyBoxTargetTypes,
                    wrapperSelector: topicForumContainer,
                }, {
                    fetchHelpers: fetchHelpers,
                    components: {
                        toast: toast
                    }
                });

            });

        });

    }





    /**
    * Init tags input
    */
    const createTopicHolders: Array<Element> = Array.from(document.getElementsByClassName('createtopicholder'));
    const editPostHolders: Array<Element> = Array.from(document.getElementsByClassName('editpostholder'));

    const createAndEditHolders: Array<Element> = [...createTopicHolders, ...editPostHolders];

    if (createAndEditHolders?.length > 0) {

        import('@modules/ui/components/createTopic').then(({ CreateTopic }) => {

            createAndEditHolders.forEach((createAndEditHolder: HTMLFormElement) => {

                new CreateTopic({
                    wrapperSelector: createAndEditHolder
                }, {
                    components: {
                        toast: toast
                    }
                });

            });

        });

    }

    /**
    * Dialog 
    */
    const dialogTriggers: Array<Element> = Array.from(document.getElementsByClassName('js-dialog'));

    if (dialogTriggers?.length > 0) {

        import('@modules/ui/components/dialog').then(({ Dialog }) => {

            dialogTriggers.forEach((dialogTrigger: HTMLElement) => {

                new Dialog({
                    wrapperSelector: dialogTrigger
                });

            });

        });

    }


    /**
    * load more buttons
    */
    const loadMoreButtons: Array<Element> = Array.from(document.getElementsByClassName('js-loadmore'));

    if (loadMoreButtons?.length > 0) {

        Promise.all([
            import('@modules/ui/components/loadMoreButton'),
            import('@modules/ui/components/topicPost')
        ])
            .then(([{ LoadMoreButton }, { TopicPost }]) => {

                loadMoreButtons.forEach((loadMoreButton: HTMLButtonElement) => {

                    const requestId = loadMoreButton.getAttribute('data-request-id');
                    const appendTargetId: string = loadMoreButton.getAttribute('data-target-id');
                    const endpointType: string = loadMoreButton.getAttribute('data-endpoint-type');
                    const appendTargetElement: HTMLElement = document.getElementById(appendTargetId);
                    const maximRequests = parseInt(loadMoreButton.getAttribute('data-maxim-requests'));
                    const requestIndex = parseInt(loadMoreButton.getAttribute('data-request-index')) + 1;

                    const getFetchUrl = (requestIndex: number): string => {

                        const endpoints = {
                            'getPostComments': `/topic/ajaxmoreposts/?TopicId=${requestId}&PageIndex=${requestIndex}`,
                            'getLatestTopics': `/group/LoadMoreTopics/?groupId=${requestId}&p=${requestIndex}`
                        };

                        return endpoints[endpointType];

                    }

                    const topicPostPlaceholder = new TopicPost({
                        wrapperSelector: (document as any)
                    }, {
                        fetchHelpers: fetchHelpers,
                        components: {
                            toast: toast
                        }
                    });

                    const requestSuccesCallbacks = {
                        'getPostComments': topicPostPlaceholder.bindFeaturesToPost
                    }

                    const requestSuccessCallback = requestSuccesCallbacks[endpointType];

                    new LoadMoreButton({
                        getFetchUrl: getFetchUrl,
                        requestIndex: requestIndex,
                        maximRequests: maximRequests,
                        wrapperSelector: loadMoreButton,
                        appendTargetElement: appendTargetElement,
                        requestSuccessCallback: requestSuccessCallback
                    }, {
                        fetchHelpers: fetchHelpers
                    });

                });

            });

    }


    /**
    * tinyMCE components
    */
    const tinyMCEContainers: Array<Element> = Array.from(document.getElementsByClassName('js-tinyMCE-container'));

    if (tinyMCEContainers?.length > 0) {

        import('@modules/ui/components/tinyMCE').then(({ TinyMCE }) => {

            tinyMCEContainers.forEach((tinyMCEContainer: HTMLElement) => {

                const tinyMceTextarea = <HTMLTextAreaElement>tinyMCEContainer.getElementsByClassName('js-tinyMCE')[0];
                const tinyMceId : string = tinyMceTextarea.getAttribute('id');
                const tinyMceAPI: any = window.tinyMCE.get(tinyMceId);
                const notEmpty = (value: string): boolean => Boolean(value);

                const validators = {
                    'notEmpty' :  notEmpty
                }

                new TinyMCE({
                    validators: validators,
                    tinyMceAPI: tinyMceAPI,
                    wrapperSelector: tinyMCEContainer,
                });

            });

        });

    }



}