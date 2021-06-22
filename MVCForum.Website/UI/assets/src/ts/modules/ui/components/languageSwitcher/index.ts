import { UIComponentBase } from '@modules/ui/componentBase';
import * as fetchHelpers from '@utilities/fetch';
import { FetchOptions } from '@appTypes/fetch';

/**
 * Language switcher
 */
export class LanguageSwitcher extends UIComponentBase {

    wrapperSelector: HTMLSelectElement = undefined;
    apiUrl: string = undefined;
    fetchHelpers: any = undefined;

    constructor(config: {
        wrapperSelector: HTMLSelectElement,
        apiUrl?: string
    }, dependencies = {
        fetchHelpers: fetchHelpers
    }) {

        super(config, dependencies);

        this.wrapperSelector = config.wrapperSelector;
        this.apiUrl = config.apiUrl ?? '/Language/ChangeLanguage';
        this.fetchHelpers = dependencies.fetchHelpers;

        this.handleRequest = this.handleRequest.bind(this);

        this.wrapperSelector.addEventListener('change', () => this.handleRequest());

    }

    private handleRequest = (): void => {

        const selectedLanguage: string = this.wrapperSelector.value;            
        const { setFetchOptions, fetchData } = this.fetchHelpers;
        const fetchOptions: FetchOptions = setFetchOptions('POST', {}, '', { 
            lang: selectedLanguage 
        });

        fetchData(this.apiUrl, fetchOptions, 60000)
            .then(() => this.emit('success'))
            .catch((error: any) => this.emit('error', `Error: ${error}`));

    }

}
