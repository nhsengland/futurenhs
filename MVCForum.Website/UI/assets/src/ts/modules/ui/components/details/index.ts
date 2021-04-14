import { UIComponentBase } from '@modules/ui/componentBase/index';
import { Icon } from '@modules/ui/components/icon';

/**
 * Progressive enhancement for HTML5 details element
 */
export class Details extends UIComponentBase {

    selector: HTMLDetailsElement = undefined;
    summary: HTMLElement = undefined;
    summaryIcon: SVGSVGElement = undefined;
    icon: Icon = undefined;
    openIcon: string = undefined;
    closedIcon: string = undefined;

    constructor(selector: HTMLDetailsElement) {

        super();

        this.selector = selector;
        this.summary = this.selector.querySelector('summary');
        this.summaryIcon = this.summary?.querySelector('svg:last-child');
        this.openIcon = this.summary?.dataset?.openIcon;
        this.closedIcon = this.summary?.dataset?.closedIcon;

        /**
         * Create an Icon instance if icon config provided
         */
        if(this.summaryIcon && this.openIcon && this.closedIcon){

            this.icon = new Icon(this.summaryIcon, this.closedIcon);

        }

        const isOpen: boolean = this.selector.hasAttribute('open');
        const iconToUse: string = isOpen ? this.openIcon : this.closedIcon;

        /**
         * Set the initial icon state
         */
        this.icon?.update(iconToUse);

        /**
         * Handle toggle events
         */
        this.selector?.addEventListener('toggle', () => {

            const isOpen: boolean = this.selector.hasAttribute('open');
            const iconToUse: string = isOpen ? this.openIcon : this.closedIcon;

            this.icon?.update(iconToUse);

        });

    }

}
