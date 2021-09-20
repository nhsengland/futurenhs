import { UIComponentBase } from '@modules/ui/componentBase';
import * as fetchHelpers from '@utilities/fetch';
import { FetchOptions } from '@appTypes/fetch';

/**
 * Load more button
 */
export class LoadMoreButton extends UIComponentBase {

    method: 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE' = 'POST';
    dataType: string = 'html';
    getFetchUrl: Function = undefined;
    requestIndex: number = undefined;
    contentType: string = 'text/html';
    maximRequests: number = undefined;
    fetchHelpers: typeof fetchHelpers = undefined;
    wrapperSelector: HTMLButtonElement = undefined;
    

    constructor(config: {
        dataType?: string; 
        contentType?: string;
        requestIndex: number;
        maximRequests: number;
        getFetchUrl: Function;
        appendTargetElement: Element
        requestSuccessCallback?: Function;
        wrapperSelector: HTMLButtonElement;
        method?: 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE';
    }, dependencies = {
        fetchHelpers: fetchHelpers
    }) {

        super(config, dependencies);

        this.getFetchUrl = config.getFetchUrl;
        this.method = config.method ?? this.method;
        this.dataType = config.dataType ?? this.dataType;
        this.contentType = config.contentType ?? this.contentType;
        this.requestIndex = config.requestIndex;
        this.maximRequests = config.maximRequests;
        this.fetchHelpers = dependencies.fetchHelpers;
        this.wrapperSelector = config.wrapperSelector;
        this.appendTargetElement = config.appendTargetElement;
        this.requestSuccessCallback = config.requestSuccessCallback;

        this.init();

        return this;

    }

    public init = (): void => {

        let requestIndex = this.requestIndex;
        const requestSuccessCallback = this.requestSuccessCallback;
        
        if( requestIndex > this.maximRequests) {
            return;
        }


        this.wrapperSelector.addEventListener('click', (e: Event) => {

            e?.preventDefault();

            const dataType = this.dataType;
            const contentType = this.contentType;
            const { setFetchOptions, fetchData } = this.fetchHelpers;
            const fetchOptions: FetchOptions = setFetchOptions({
                method: this.method,
                contentType: contentType
            });
                
            fetchData({
                url: this.getFetchUrl(requestIndex),
                options: fetchOptions,
                timeOut: 60000,
                dataType: dataType
            })
            .then((html: string) => {
                
                this.appendTargetElement.insertAdjacentHTML('beforeend', html);
                
                requestSuccessCallback?.();

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
