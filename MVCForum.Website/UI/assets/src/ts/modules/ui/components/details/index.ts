import { UIComponentBase } from '@modules/ui/componentBase/index';
import { Icon } from '@modules/ui/components/icon';

/**
 * Progressive enhancement for HTML5 details element
 */
export class Details extends UIComponentBase {

    wrapperSelector: HTMLDetailsElement = undefined;
    summary: HTMLElement = undefined;
    summaryIcon: SVGSVGElement = undefined;
    icon: Icon = undefined;
    openIcon: string = undefined;
    closedIcon: string = undefined;

    constructor(config: {
        wrapperSelector: HTMLDetailsElement
    }) {

        super(config);

        this.wrapperSelector = config.wrapperSelector;
        this.summary = this.wrapperSelector.querySelector('summary');
        this.summaryIcon = this.summary?.querySelector('svg:last-child');
        this.openIcon = this.summary?.dataset?.openIcon;
        this.closedIcon = this.summary?.dataset?.closedIcon;

        /**
         * Create an Icon instance if icon config provided
         */
        if(this.summaryIcon && this.openIcon && this.closedIcon){

            this.icon = new Icon({
                wrapperSelector: this.summaryIcon,
                name: this.closedIcon
            });

        }

        const isOpen: boolean = this.wrapperSelector.hasAttribute('open');
        const iconToUse: string = isOpen ? this.openIcon : this.closedIcon;

        /**
         * Set the initial icon state
         */
        this.icon?.update(iconToUse);

        /**
         * Handle toggle events
         */
         this.wrapperSelector?.addEventListener('toggle', () => {

            const isOpen: boolean = this.wrapperSelector.hasAttribute('open');
            const iconToUse: string = isOpen ? this.openIcon : this.closedIcon;

            this.icon?.update(iconToUse);

        });

    }

}
