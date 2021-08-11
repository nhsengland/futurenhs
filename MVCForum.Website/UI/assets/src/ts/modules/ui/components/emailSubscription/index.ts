import { UIComponentBase } from '@modules/ui/componentBase/index';
import { Toast } from '@modules/ui/components/toast';

/**
 * Email subscription
 */
export class EmailSubscription extends UIComponentBase {

    toast: Toast = undefined;
    wrapperSelector: HTMLElement = undefined;

    constructor(config: {
        wrapperSelector: HTMLElement
    }, dependencies: {
        components: {
            toast: Toast
        }
    }) {

        super(config, dependencies);

        this.wrapperSelector = config.wrapperSelector;
        this.toast = dependencies.components.toast;

        /*------------ TODO: REFACTOR --------------------*/
        const emailsubscription = () => {

            var esub = $(".emailsubscription");
            if (esub.length > 0) {
                esub.click((e) => {
                    e.preventDefault();
                    var entityId = $(this).data("id");
                    var subscriptionType = $(this).data("type");

                    $(this).hide();

                    var subscribeEmailViewModel: any = new Object();
                    subscribeEmailViewModel.Id = entityId;
                    subscribeEmailViewModel.SubscriptionType = subscriptionType;

                    // Ajax call to post the view model to the controller
                    var strung = JSON.stringify(subscribeEmailViewModel);

                    $.ajax({
                        url: window.app_base + "Email/Subscribe",
                        type: "POST",
                        cache: false,
                        data: strung,
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            $(".emailunsubscription").fadeIn();
                        },
                        error: (xhr, ajaxOptions, thrownError) => {
                            this.toast.show("Error: " + xhr.status + " " + thrownError);
                        }
                    });
                });
            }
        };

        const emailunsubscription = () => {

            const eunsub: JQuery = $(".emailunsubscription");

            if (eunsub.length > 0) {

                eunsub.on('click', (event: any) => {

                    event.preventDefault();

                    const thisLink: any = $(this);
                    const entityId: string | number = $(this).data("id");
                    const subscriptionType: string = $(this).data("type");
                    const unSubscribeEmailViewModel: any = new Object();

                    thisLink.hide();

                    unSubscribeEmailViewModel.Id = entityId;
                    unSubscribeEmailViewModel.SubscriptionType = subscriptionType;

                    // Ajax call to post the view model to the controller
                    const strung: string = JSON.stringify(unSubscribeEmailViewModel);

                    $.ajax({
                        url: window.app_base + "Email/UnSubscribe",
                        type: "POST",
                        cache: false,
                        data: strung,
                        contentType: "application/json; charset=utf-8",
                        success: (data) => {

                            // We might be on the following page, so hide the items we need
                            var categoryRow = thisLink.closest(".categoryrow");
                            if (categoryRow) {
                                categoryRow.fadeOut("fast");
                            }
                            var topicrow = thisLink.closest(".topicrow");
                            if (topicrow) {
                                topicrow.fadeOut("fast");
                            }

                            var emailsubscription = $(".emailsubscription");
                            if (emailsubscription) {
                                emailsubscription.fadeIn();
                            }
                        },
                        error: (xhr, ajaxOptions, thrownError) => {
                            this.toast?.show("Error: " + xhr.status + " " + thrownError);
                        }
                    });
                });
            }

        };

        emailsubscription();
        emailunsubscription();

    }

}
