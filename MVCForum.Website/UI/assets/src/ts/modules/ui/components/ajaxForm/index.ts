import { UIComponentBase } from '@modules/ui/componentBase/index';
import * as fetchHelpers from '@utilities/fetch';
import { FetchOptions } from '@appTypes/fetch';

/**
 * Ajax Progressive enhancement for form element
 */
export class AjaxForm extends UIComponentBase {

    wrapperSelector: HTMLFormElement = undefined;

    constructor(config: {
        wrapperSelector: HTMLFormElement,
    }, dependencies = {
        fetchHelpers: fetchHelpers
    }) {

        super(config, dependencies);

        this.wrapperSelector = config.wrapperSelector;
        this.wrapperSelector.addEventListener('submit', (event: any) => {

            event.preventDefault();

            ($(this) as any).validate();

            const isValid: boolean = ($(this) as any).valid();

            if (isValid) {

                const { setFetchJSONOptions, fetchJSON } = dependencies.fetchHelpers;
                const fetchOptions: FetchOptions = setFetchJSONOptions('POST', {}, '', $(this).serialize());

                fetchJSON(this.apiUrl, fetchOptions, 60000)
                    .then((data: any) => this.emit('success', data))
                    .catch((error: any) => this.emit('error', `Error: ${error}`));

            }

        });

    }

}
