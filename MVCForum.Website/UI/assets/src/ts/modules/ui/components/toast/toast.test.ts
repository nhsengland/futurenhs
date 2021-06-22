import { Toast } from './index';

beforeEach(() => {

    document.body.innerHTML = `
        <div id="main">
            <div class="js-toast"></div>
        </div>`;

});

describe('Toast', () => {

    it('Binds successfully', () => {

        const selector: HTMLElement = document.querySelector('.js-toast');
        const toast: Toast = new Toast({
            wrapperSelector: selector
        });

        expect(toast).toBeTruthy();

    });

    it('Shows nothing on init by default', () => {

        const selector: HTMLElement = document.querySelector('.js-toast');
        const toast: Toast = new Toast({
            wrapperSelector: selector
        });

        expect(selector.innerHTML.length).toEqual(0);

    });

    it('Shows toast on init when provided with content in config', () => {

        const selector: HTMLElement = document.querySelector('.js-toast');
        const toast: Toast = new Toast({
            wrapperSelector: selector,
            messageText: 'mockText'
        });

        expect(selector.innerHTML.length).not.toEqual(0);

    });

    it('Shows the expected content inside the toast when provided with content in config and when set via public method', () => {

        const selector: HTMLElement = document.querySelector('.js-toast');
        const toast: Toast = new Toast({
            wrapperSelector: selector,
            messageText: 'mockText1'
        });

        expect(selector.innerHTML.includes('mockText1')).toBeTruthy();
        expect(selector.innerHTML.includes('mockText2')).toBeFalsy();

        toast.show('mockText2');

        expect(selector.innerHTML.includes('mockText1')).toBeFalsy();
        expect(selector.innerHTML.includes('mockText2')).toBeTruthy();

    });

    it('Hides toast after the prescribed timeout', (done) => {

        const selector: HTMLElement = document.querySelector('.js-toast');
        const toast: Toast = new Toast({
            wrapperSelector: selector,
            messageText: 'mockText',
            timeOutMillis: 500
        });

        expect(selector.innerHTML.length).not.toEqual(0);

        setTimeout(() => {

            expect(selector.innerHTML.length).toEqual(0);
            done();

        }, 501);

    });

});

