import { debounce } from 'debounce';
import { UIComponentBase } from '@modules/ui/componentBase/index';

/**
 * Provides dynamic behaviours for the site header
 */
export class SiteHeaderNav extends UIComponentBase {

    wrapperSelector: HTMLElement = undefined;
    mobileNavTriggers: Array<HTMLDetailsElement> = undefined;
    // masterMenuNav: HTMLDetailsElement = undefined;
    parentMenuNavs: Array<HTMLDetailsElement> = undefined;

    constructor(config: {
        wrapperSelector: HTMLElement
    }, childSelectors: any = {
        mobileNavTriggers: Array.from(config.wrapperSelector.querySelectorAll('.c-site-header-nav_content')),
        // masterMenuNav: config.wrapperSelector.querySelector('.c-site-header_tools-menu'),
        parentMenuNavs: Array.from(config.wrapperSelector.querySelectorAll('.c-site-header-nav_root-nav details')),
        mobileSearchInput: document.getElementById('mobile-site-search'),
        mobileSearchTrigger: document.getElementById('mobile-search-trigger'),
    }) {

        super(config);

        this.resetNavState = this.resetNavState.bind(this);
        this.getisTablet = this.getisTablet.bind(this);
        this.getIsDesktop = this.getIsDesktop.bind(this);
        this.bindMobileNav = this.bindMobileNav.bind(this);
        this.bindSiblings = this.bindSiblings.bind(this);
        this.toggleSiblingsOnEvent = this.toggleSiblingsOnEvent.bind(this);

        this.selector = config.wrapperSelector;
        this.mobileNavTriggers = childSelectors.mobileNavTriggers;
        // this.masterMenuNav = childSelectors.masterMenuNav;
        this.parentMenuNavs = childSelectors.parentMenuNavs;
        this.mobileSearchInput = childSelectors.mobileSearchInput;
        this.mobileSearchTrigger = childSelectors.mobileSearchTrigger;

        this.bindMobileNav();
        this.bindSiblings(this.mobileNavTriggers);
        this.bindSiblings(this.parentMenuNavs);
        this.resetNavState();
        console.log(this.css.breakPoints)

        /**
         * On window resize ensure the main nav is reset if not in mobile breakpoint
         */
        window.addEventListener('resize', debounce(() => {

            this.resetNavState();

        }, 50, undefined));

        /**
         * On click or focus outside navs, close the navs
         */
        window.addEventListener('click', this.toggleSiblingsOnEvent);
        document.addEventListener('focusin', this.toggleSiblingsOnEvent);
        // document.addEventListener('scroll', () => !this.getIsDesktop() && this.masterMenuNav.removeAttribute('open'));

    }

    /**
     * Updates nav and body scroll state according to ctive breakpoint
     */
    private resetNavState: Function = (): void => {

        const isTablet: boolean = this.getisTablet();
        const isDesktop: boolean = this.getIsDesktop();

        this.setFixedBackground(false);

        // if(isDesktop){

        //     this.masterMenuNav.setAttribute('open', '');

        // } else {

        //     this.masterMenuNav.removeAttribute('open');

        // }

        this.mobileNavTriggers.forEach((mobileNavTrigger) => {

            // mobileNavTrigger.removeAttribute('open');

            if(isDesktop){

                mobileNavTrigger.setAttribute('open', '');

            } else {

                mobileNavTrigger.removeAttribute('open');

            }

        });

        // if((this.mobileSearchInput === document.activeElement) && isTablet) {

        //     this.mobileSearchTrigger.setAttribute('open', '');

        // }

    }

    /**
     * Returns whether current window width is within mobile breakpoint
     */
    private getisTablet: Function = (): boolean => {

        return window.innerWidth <= this.css.breakPoints.desktop;

    }

    /**
     * Returns whether current window width is above large desktop breakpoint
     */
    private getIsDesktop: Function = (): boolean => {

        return window.innerWidth > this.css.breakPoints.desktop;

    }

    /**
     * Optionally locks scroll on body element - used when mobile nav overlay is visible
     */
    private setFixedBackground: Function = (shouldFix: boolean): void => {

        if(shouldFix){

            document
                .querySelector('body')
                .classList
                .add('u-overflow-hidden');

        } else {

            document
                .querySelector('body')
                .classList
                .remove('u-overflow-hidden');

        }

    }

    /**
     * Handles toggle events from the mobile nav trigger
     */
    private bindMobileNav: Function = (): void => {

        this.mobileNavTriggers.forEach((mobileNavTrigger) => {

            mobileNavTrigger.addEventListener('toggle', () => {

                const isTablet: boolean = this.getisTablet();
                const isOpen: boolean = mobileNavTrigger.hasAttribute('open');

                if(isTablet){

                    this.setFixedBackground(isOpen);

                }

            });

        });

    }

    /**
     * Handles collapsing nav siblings when a nav item is opened
     */
    private bindSiblings: Function = (siblings: Array<HTMLDetailsElement>): void => {

        siblings.forEach((thisDetail: HTMLDetailsElement) => {

            thisDetail.addEventListener('click', () => {

                siblings.forEach((detail: HTMLDetailsElement) => {

                    if (detail !== thisDetail) {

                        detail.removeAttribute('open');

                    }

                });

            });

        });

    }

    /**
     * Handles collapsing nav siblings in events
     */
    private toggleSiblingsOnEvent: any = (event: any): void => {

        const isTablet: boolean = this.getisTablet();
        // const isClickInMasterMenuNav: boolean = this.masterMenuNav.contains(event.target);

        // if(!isClickInMasterMenuNav && !this.getIsDesktop()){

        //     this.masterMenuNav.removeAttribute('open');

        // }

        this.mobileNavTriggers.forEach((mobileNavTrigger) => {

            const isClickInElement: boolean = mobileNavTrigger.contains(event.target);

            if(!isClickInElement && isTablet){

                mobileNavTrigger.removeAttribute('open');

            }

        });

        this.parentMenuNavs.forEach((parentMenuNav) => {

            const isClickInElement: boolean = parentMenuNav.contains(event.target);

            if(!isClickInElement){

                parentMenuNav.removeAttribute('open');

            }

        });

    }

}
