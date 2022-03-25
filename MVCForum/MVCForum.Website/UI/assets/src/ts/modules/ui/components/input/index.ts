import { UIComponentBase } from '@modules/ui/componentBase';

export class Input extends UIComponentBase {

    wrapperSelector: HTMLElement = undefined;
    focusValidators: {
        [key: string]: Function
    } = {};

    constructor(config: {
        wrapperSelector: HTMLElement;
        focusValidators?: {
            [key: string]: Function
        };
    }) {

        super(config);

        this.wrapperSelector = config.wrapperSelector;
        this.focusValidators = config.focusValidators;

        this.init();

        return this;

    }

    init = (): void => {

        this.bindFocusValidation();

    }

    bindFocusValidation = (): void => {

        const wrapperSelector = this.wrapperSelector;
        const input = <HTMLInputElement>wrapperSelector.getElementsByClassName('js-input')[0];
        const inputHolder = wrapperSelector.getElementsByClassName('js-input-holder')[0];
        const requirements: Array<string> = input.getAttribute('data-focus-requirements')?.split(',');

        const handleFocus = (e: any) => {
            
            let hasErrors: boolean = false;

            requirements?.forEach((requirement) => {

                const validator = this.focusValidators[requirement];

                if(!validator){
                    return;
                }

                const currentContent: string = e.target.value;

                
                const isPassing: boolean = validator(currentContent);
                const errorMessage: Element = wrapperSelector.getElementsByClassName(`js-input-error-${requirement}`)?.[0];

                if (!isPassing) {
                    hasErrors = true;
                    errorMessage.classList.remove('u-hidden');
                } else {
                    errorMessage.classList.add('u-hidden');
                }               

            });

            if (hasErrors) {

                input.classList.add('c-input--error');
                inputHolder.classList.add('c-form-group--error');
                return;
            }

            input.classList.remove('c-input--error');
            inputHolder.classList.remove('c-form-group--error');

        }

        input?.addEventListener('focusout', handleFocus);

    }


}
