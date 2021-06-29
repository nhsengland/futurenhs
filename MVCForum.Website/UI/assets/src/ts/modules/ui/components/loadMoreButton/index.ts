import { UIComponentBase } from '@modules/ui/componentBase';
import * as fetchHelpers from '@utilities/fetch';
import { FetchOptions } from '@appTypes/fetch';

/**
 * Load more button
 */
export class LoadMoreButton extends UIComponentBase {

    method: string = 'POST';
    getFetchUrl: Function = undefined;
    requestIndex: number = undefined;
    contentType: string = 'text/html';
    maximRequests: number = undefined;
    wrapperSelector: HTMLButtonElement = undefined;

    constructor(config: {
        method?: string;
        getFetchUrl: Function;
        contentType?: string;
        requestIndex: number;
        maximRequests: number;
        wrapperSelector: HTMLButtonElement;
        appendTargetElement: Element
    }, dependencies = {
        fetchHelpers: fetchHelpers
    }) {

        super(config, dependencies);

        this.getFetchUrl = config.getFetchUrl;
        this.method = config.method ?? this.method;
        this.contentType = config.contentType ?? this.contentType;
        this.requestIndex = config.requestIndex;
        this.maximRequests = config.maximRequests;
        this.fetchHelpers = dependencies.fetchHelpers;
        this.wrapperSelector = config.wrapperSelector;
        this.appendTargetElement = config.appendTargetElement;

        this.init();

        return this;

    }

    public init = (): void => {

        let requestIndex = this.requestIndex;
        
        if( requestIndex > this.maximRequests) {
            return;
        }

        this.wrapperSelector.classList.remove('u-hidden');

        this.wrapperSelector.addEventListener('click', (e: Event) => {

            e?.preventDefault();

            const contentType = this.contentType;
            const { setFetchOptions, fetchData } = this.fetchHelpers;
            const fetchOptions: FetchOptions = setFetchOptions({
                method: this.method,
                contentType: contentType
            });
                
            fetchData({
                url: this.getFetchUrl(requestIndex),
                options: { ...fetchOptions, contentType },
                timeOut: 60000 
            })
            .then((html: string) => {
                
                this.appendTargetElement.insertAdjacentHTML('beforeend', html);

                if(this.maximRequests === requestIndex) {
                    
                    this.wrapperSelector.classList.add('u-hidden');

                    return;
                    
                }

                requestIndex++;

            })
            .catch((error: any) => {

                console.error(`Error: ${error}`);
                return null;

            });


        })

    }

}
