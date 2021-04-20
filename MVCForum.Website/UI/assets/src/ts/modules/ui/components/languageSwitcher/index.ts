import { UIComponentBase } from '@modules/ui/componentBase';
import * as fetchHelpers from '@utilities/fetch';
import { FetchOptions } from '@appTypes/fetch';

/**
 * Language switcher
 */
export class LanguageSwitcher extends UIComponentBase {

    wrapperSelector: HTMLSelectElement = undefined;
    apiUrl: string = undefined;

    constructor(config: {
        wrapperSelector: HTMLSelectElement,
        apiUrl?: string
    }, dependencies = {
        fetchHelpers: fetchHelpers
    }) {

        super();

        this.wrapperSelector = config.wrapperSelector;
        this.apiUrl = config.apiUrl ?? '/Language/ChangeLanguage';

        this.wrapperSelector.addEventListener('change', () => {

            const selectedLanguage: string = this.wrapperSelector.value;            
            const { setFetchJSONOptions, fetchJSON } = dependencies.fetchHelpers;
            const fetchOptions: FetchOptions = setFetchJSONOptions('POST', {}, '', { 
                lang: selectedLanguage 
            });

            fetchJSON(this.apiUrl, fetchOptions, 60000)
                .then(() => this.emit('success'))
                .catch((error: any) => this.emit('error', `Error: ${error}`));

        });

    }

}
