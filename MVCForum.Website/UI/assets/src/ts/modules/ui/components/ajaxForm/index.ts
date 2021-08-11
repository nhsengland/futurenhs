import { UIComponentBase } from '@modules/ui/componentBase/index';
import * as fetchHelpers from '@utilities/fetch';
import { FetchOptions } from '@appTypes/fetch';

/**
 * Ajax Progressive enhancement for form element
 */
export class AjaxForm extends UIComponentBase {

    apiUrl: string = undefined;
    wrapperSelector: HTMLFormElement = undefined;

    constructor(config: {
        wrapperSelector: HTMLFormElement,
        apiUrl?: string;
    }, dependencies = {
        fetchHelpers: fetchHelpers
    }) {

        super(config, dependencies);

        this.wrapperSelector = config.wrapperSelector;
        this.apiUrl = config.apiUrl;

        this.wrapperSelector.addEventListener('submit', (event: any) => {

            event.preventDefault();

            ($(this) as any).validate();

            const isValid: boolean = ($(this) as any).valid();

            if (isValid) {

                const { setFetchOptions, fetchData } = dependencies.fetchHelpers;
                const fetchOptions: FetchOptions = setFetchOptions({
                    method: 'POST',
                    customHeaders: {},
                    etag: '',
                    body: $(this).serialize()
                });

                fetchData({
                    url: this.apiUrl,
                    options: fetchOptions,
                    timeOut: 60000 
                })
                .then((data: any) => this.emit('success', data))
                .catch((error: any) => this.emit('error', `Error: ${error}`));

            }

        });

    }

}
