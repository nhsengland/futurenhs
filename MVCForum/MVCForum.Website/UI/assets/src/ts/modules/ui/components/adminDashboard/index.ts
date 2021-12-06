import { UIComponentBase } from '@modules/ui/componentBase';

/**
 * AdminDashboard 
 */
export class AdminDashboard extends UIComponentBase {

    fetchTargets: any = undefined;

    constructor(config: {
        fetchTargets: any;
        wrapperSelector: any;
    }) {

        super(config);

        this.fetchTargets = config.fetchTargets;

        this.bindDataToHtmlElements = this.bindDataToHtmlElements.bind(this);

    }

    public bindDataToHtmlElements = (): void => {

        for (const className in this.fetchTargets) {

            if (Object.prototype.hasOwnProperty.call(this.fetchTargets, className)) {

                const endpoint: string = this.fetchTargets[className];
                const fetchPath: string = '/Admin/Dashboard/' + endpoint;

                $.post(fetchPath,
                    function (data) {

                        $(`.${className}`).html(data);

                    });


            }
        }


    }

}
