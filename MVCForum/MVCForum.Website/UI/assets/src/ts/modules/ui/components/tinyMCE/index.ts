import { UIComponentBase } from '@modules/ui/componentBase';

export class TinyMCE extends UIComponentBase {

    wrapperSelector: HTMLElement = undefined;
    tinyMceAPI: any = undefined;
    validators: {
        [key: string]: Function
    } = {};

    constructor(config: {
        tinyMceAPI: any;
        wrapperSelector: HTMLElement;
        validators: {
            [key: string]: Function
        };
    }) {

        super(config);

        this.wrapperSelector = config.wrapperSelector;
        this.tinyMceAPI = config.tinyMceAPI;
        this.validators = config.validators;

        this.init();

        return this;

    }


    init = (): void => {

        this.submitContent();
        this.clearContent();

    }

    clearContent = (): void => {
        
        const clearContentBtn: Element = this.wrapperSelector.getElementsByClassName('js-tinyMCE-clear')?.[0];
        
        clearContentBtn?.addEventListener('click', (e: Event)=>{
            
            e.preventDefault();
            e.stopImmediatePropagation();
            this.tinyMceAPI.setContent(''); 

        });

    }

    submitContent = (): void => {
        const tinyMceAPI = this.tinyMceAPI;
        const wrapperSelector = this.wrapperSelector;
        const tinyMceHolder = wrapperSelector.getElementsByClassName('js-tinyMCE-holder')[0];
        const submitBtn: Element = wrapperSelector.getElementsByClassName('js-tinyMCE-submit')?.[0];
        const requirements: Array<string> = submitBtn.getAttribute('data-mce-requirements')?.split(',');

        const handleClick = (e: Event) => {
            
            let hasErrors: boolean = false;

            requirements?.forEach((requirement) => {

                const validator = this.validators[requirement];

                if(!validator){
                    return;
                }

                const currentContent: string = tinyMceAPI.getContent();
                const isPassing: boolean = validator(currentContent);
                const errorMessage: Element = wrapperSelector.getElementsByClassName(`js-tinyMCE-error-${requirement}`)?.[0];

                if (!isPassing) {
                    hasErrors = true;
                    errorMessage.classList.remove('u-hidden');
                } else {
                    errorMessage.classList.add('u-hidden');
                }               

            });

            if (hasErrors) {

                e.preventDefault();
                e.stopImmediatePropagation();

                tinyMceHolder.classList.add('u-has-error');

                return;
            }

            tinyMceHolder.classList.remove('u-has-error');

        }

        submitBtn?.addEventListener('click', handleClick);

    }


}
