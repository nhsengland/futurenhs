import { UIComponentBase } from '@modules/ui/componentBase';

/**
 * Mobile nav
 */
export class MobileNav extends UIComponentBase {

    triggerButtonSelector: HTMLElement = undefined;
    contentSelector: HTMLElement = undefined;

    constructor(config: {
        triggerButtonSelector?: HTMLElement,
        contentSelector?: HTMLElement
    }) {

        super();

        this.triggerButtonSelector = config.triggerButtonSelector ?? document.querySelector('.showmobilenavbar');
        this.contentSelector = config.contentSelector ?? document.querySelector('.mobilenavbar-inner ul.nav');
            
        this.triggerButtonSelector?.addEventListener('click', (event: any) => {
            
            event.preventDefault();
            
            $(this.contentSelector).slideToggle();

        });

        return this;

    }

}
