import { UIComponentBase } from '@modules/ui/componentBase';

/**
 * Mobile nav
 */
export class MobileNav extends UIComponentBase {

    triggerButtonSelector: HTMLElement = undefined;
    contentSelector: HTMLElement = undefined;

    constructor(config: {
        wrapperSelector: HTMLElement;
        triggerButtonSelector: HTMLElement;
        contentSelector: HTMLElement;
    }) {

        super(config);

        this.triggerButtonSelector = config.triggerButtonSelector;
        this.contentSelector = config.contentSelector;
            
        this.triggerButtonSelector?.addEventListener('click', (event: any) => {
            
            event.preventDefault();
            
            $(this.contentSelector).slideToggle();

        });

        return this;

    }

}
