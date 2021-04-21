import { AjaxForm } from './index';

let fetchHelpersMock = undefined;

beforeEach(() => {

    (jQuery.fn as any).validate = jest.fn();
    (jQuery.fn as any).valid = jest.fn(() => true);

    fetchHelpersMock = {
        setFetchJSONOptions: jest.fn(),
        fetchJSON: jest.fn(() => Promise.resolve({})),
        fetchWithTimeOut: jest.fn(),
        getErrorMessageString: jest.fn(() => 'Error')    
    }

    document.body.innerHTML = `
        <div id="main">
            <form class="js-ajax-form">
                <label>Mock</label>
                <select>
                    <option value="">Please select an option</option>
                    <option value="mock1">Moving home</option>
                    <option value="mock2">Report a leak</option>
                </select>
                <button type="submit">Submit</button>
            </form>
        </div>`;

});

describe('Ajax form', () => {

    it('Binds successfully', () => {

        const selector: HTMLFormElement = document.querySelector('.js-ajax-form');
        const ajaxForm: AjaxForm = new AjaxForm({
            wrapperSelector: selector
        });

        expect(ajaxForm).toBeTruthy();

    });

    it('Creates a fetch call config and makes a fetch call correctly when the form is valid', () => {

        const selector: HTMLFormElement = document.querySelector('.js-ajax-form');
        const submitButton: HTMLElement = document.querySelector('button');
        const ajaxForm: AjaxForm = new AjaxForm({
            wrapperSelector: selector
        }, {
            fetchHelpers: fetchHelpersMock
        });

        submitButton.click();

        expect(fetchHelpersMock.setFetchJSONOptions).toHaveBeenCalledTimes(1);
        expect(fetchHelpersMock.fetchJSON).toHaveBeenCalledTimes(1);

    });

    it('Emits an event on form subission success', (done) => {

        const handleSuccess: Function = jest.fn();
        const selector: HTMLFormElement = document.querySelector('.js-ajax-form');
        const submitButton: HTMLElement = document.querySelector('button');
        const ajaxForm: AjaxForm = new AjaxForm({
            wrapperSelector: selector
        }, {
            fetchHelpers: fetchHelpersMock
        });

        ajaxForm.on('success', handleSuccess);

        submitButton.click();

        setTimeout(() => {

            expect(handleSuccess).toHaveBeenCalledTimes(1);
            done();

        }, 1);

    });

    it('Emits an event on form subission error', (done) => {

        const handleError: Function = jest.fn();
        const selector: HTMLFormElement = document.querySelector('.js-ajax-form');
        const submitButton: HTMLElement = document.querySelector('button');

        fetchHelpersMock.fetchJSON = jest.fn(() => Promise.reject({}));

        const ajaxForm: AjaxForm = new AjaxForm({
            wrapperSelector: selector
        }, {
            fetchHelpers: fetchHelpersMock
        });

        ajaxForm.on('error', handleError);

        submitButton.click();

        setTimeout(() => {

            expect(handleError).toHaveBeenCalledTimes(1);
            done();

        }, 1);

    });

    it('Does nothing when the form is not valid', () => {

        (jQuery.fn as any).valid = jest.fn(() => false);

        const selector: HTMLFormElement = document.querySelector('.js-ajax-form');
        const submitButton: HTMLElement = document.querySelector('button');
        const ajaxForm: AjaxForm = new AjaxForm({
            wrapperSelector: selector
        }, {
            fetchHelpers: fetchHelpersMock
        });

        submitButton.click();

        expect(fetchHelpersMock.setFetchJSONOptions).toHaveBeenCalledTimes(0);
        expect(fetchHelpersMock.fetchJSON).toHaveBeenCalledTimes(0);

    });

});

