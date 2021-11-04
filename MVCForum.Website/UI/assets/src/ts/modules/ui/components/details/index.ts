import { debounce } from 'debounce';
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
    openOnDesktop: boolean = false;
    

    constructor(config: {
        wrapperSelector: HTMLDetailsElement
    }) {

        super(config);

        this.wrapperSelector = config.wrapperSelector;
        this.summary = this.wrapperSelector.querySelector('summary');
        this.summaryIcon = this.summary?.querySelector('svg:last-child');
        this.openIcon = this.summary?.dataset?.openIcon;
        this.closedIcon = this.summary?.dataset?.closedIcon;
        this.shouldOpenOnDesktop = this.summary?.dataset?.openOnDesktop;
        this.getIsDesktop = this.getIsDesktop.bind(this);
        this.resetOpenState = this.resetOpenState.bind(this);


        if(this.shouldOpenOnDesktop){
            this.resetOpenState();

            window.addEventListener('resize', debounce(() => {

                this.resetOpenState();

            }, 50, undefined));
        }

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

    /**
     * Returns whether current window width is above large desktop breakpoint
     */
     private getIsDesktop: Function = (): boolean => {

        return window.innerWidth >= this.css.breakPoints.desktop;

    }

    /**
     * Updates open attribute based on active breakpoint
     */
    private resetOpenState: Function = (): void => {
        const isDesktop: boolean = this.getIsDesktop();


        if(isDesktop){

            this.wrapperSelector.setAttribute('open', '');

        } else {

            this.wrapperSelector.removeAttribute('open');

        }
    }

}
