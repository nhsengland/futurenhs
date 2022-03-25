import { hasPrefersReducedMotionEnabled, isSmoothScrollSupported, fancyScrollTo, lockPageScroll } from './index';

describe('CSS helpers: hasPrefersReducedMotionEnabled', () => {

    it('Correctly returns whether the user has enabled prefersReducedMotion', () => {

        Object.defineProperty(window, 'matchMedia', {
            writable: true,
            value: jest.fn().mockImplementation(query => ({
                matches: true
            })),
        });

        expect(hasPrefersReducedMotionEnabled()).toBe(true);

        Object.defineProperty(window, 'matchMedia', {
            writable: true,
            value: jest.fn().mockImplementation(query => ({
                matches: false
            })),
        });

        expect(hasPrefersReducedMotionEnabled()).toBe(false);

    });

});

describe('CSS helpers: isSmoothScrollSupported', () => {

    it('Correctly returns whether the browser supports smoothScroll', () => {

        expect(isSmoothScrollSupported()).toBe(true);

    });

});

describe('CSS helpers: fancyScrollTo', () => {

    it('Conditionally uses smoothScroll when supported and the user does not have prefersReducedMotion enabled', () => {

        const spyScrollTo = jest.fn();
        Object.defineProperty(global.window, 'scrollTo', { value: spyScrollTo });

        fancyScrollTo(0, {
            isSmoothScrollSupported: () => true,
            hasPrefersReducedMotionEnabled: () => false
        });

        expect(spyScrollTo).toBeCalledWith({
            top: 0,
            left: 0,
            behavior: 'smooth'
        });

        fancyScrollTo(0, {
            isSmoothScrollSupported: () => false,
            hasPrefersReducedMotionEnabled: () => false
        });

        expect(spyScrollTo).toBeCalledWith(0, 0);

    });

});

describe('CSS helpers: lockPageScroll', () => {

    it('Conditionally sets the document style when the page is locked from scrolling', () => {

        let htmlStyle: any = undefined;
        let bodyStyle: any = undefined;

        lockPageScroll(true);

        htmlStyle = document.documentElement.style;
        bodyStyle = document.body.style;

        expect(htmlStyle.overflow).toEqual('hidden');
        expect(bodyStyle.overflow).toEqual('hidden');

        lockPageScroll(false);

        htmlStyle = document.documentElement.style;
        bodyStyle = document.body.style;

        expect(htmlStyle.overflow).toEqual('');
        expect(bodyStyle.overflow).toEqual('');

    });

});
