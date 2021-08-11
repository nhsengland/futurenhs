import { MobileNav } from './index';

beforeEach(() => {

    (jQuery.fn as any).slideToggle = jest.fn();

    document.body.innerHTML = `
        <div id="main">
            <button class="js-mobile-nav-button">Click me</button>
            <div class="js-mobile-nav-content">Content</div>
        </div>`;

});

describe('Mobile nav', () => {

    it('Binds successfully', () => {

        const triggerButtonSelector: HTMLElement = document.querySelector('.js-mobile-nav-button');
        const contentSelector: HTMLElement = document.querySelector('.js-mobile-nav-content');
        const mobileNav: MobileNav = new MobileNav({
            wrapperSelector: undefined,
            triggerButtonSelector: triggerButtonSelector,
            contentSelector: contentSelector
        });

        expect(mobileNav).toBeTruthy();

    });

    it('Calls slideToggle on content on button click', () => {

        const triggerButtonSelector: HTMLElement = document.querySelector('.js-mobile-nav-button');
        const contentSelector: HTMLElement = document.querySelector('.js-mobile-nav-content');
        const mobileNav: MobileNav = new MobileNav({
            wrapperSelector: undefined,
            triggerButtonSelector: triggerButtonSelector,
            contentSelector: contentSelector
        });

        expect((jQuery.fn as any).slideToggle).toHaveBeenCalledTimes(0);

        triggerButtonSelector.click();

        expect((jQuery.fn as any).slideToggle).toHaveBeenCalledTimes(1);

    });

});

