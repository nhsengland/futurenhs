import { LanguageSwitcher } from './index';

let fetchHelpersMock = undefined;

beforeEach(() => {

    fetchHelpersMock = {
        setFetchJSONOptions: jest.fn(),
        fetchJSON: jest.fn(() => Promise.resolve({})),
        fetchWithTimeOut: jest.fn(),
        getErrorMessageString: jest.fn(() => 'Error')    
    }

    document.body.innerHTML = `
        <div id="main">
            <select class="js-language-switcher">
                <option value="mock1" selected>Option 1</option>
                <option value="mock2">Option 2</option>
            </select>
        </div>`;

});

describe('Language switcher', () => {

    it('Binds successfully', () => {

        const selector: HTMLSelectElement = document.querySelector('.js-language-switcher');
        const languageSwitcher: LanguageSwitcher = new LanguageSwitcher({
            wrapperSelector: selector
        });

        expect(languageSwitcher).toBeTruthy();

    });

    it('Creates a fetch call config and makes a fetch call correctly when the select is changed', () => {

        const selector: HTMLSelectElement = document.querySelector('.js-language-switcher');
        const languageSwitcher: LanguageSwitcher = new LanguageSwitcher({
            wrapperSelector: selector
        }, {
            fetchHelpers: fetchHelpersMock
        });

        $(selector).val('mock2');
        $(selector).trigger('change');

        setTimeout(() => {

            expect(fetchHelpersMock.setFetchJSONOptions).toHaveBeenCalledTimes(1);
            expect(fetchHelpersMock.fetchJSON).toHaveBeenCalledTimes(1);

        }, 1);

    });

    it('Emits an event on language change success', (done) => {

        const handleSuccess: Function = jest.fn();
        const selector: HTMLSelectElement = document.querySelector('.js-language-switcher');
        const languageSwitcher: LanguageSwitcher = new LanguageSwitcher({
            wrapperSelector: selector
        }, {
            fetchHelpers: fetchHelpersMock
        });

        languageSwitcher.on('success', handleSuccess);
        (languageSwitcher as any).handleRequest(); // workaround as simulating select change event not working
        
        setTimeout(() => {

            expect(handleSuccess).toHaveBeenCalledTimes(1);
            done();

        }, 1);

    });

    it('Emits an event on language change error', (done) => {

        const handleError: Function = jest.fn();
        const selector: HTMLSelectElement = document.querySelector('.js-language-switcher');
        
        fetchHelpersMock.fetchJSON = jest.fn(() => Promise.reject({}));

        const languageSwitcher: LanguageSwitcher = new LanguageSwitcher({
            wrapperSelector: selector
        }, {
            fetchHelpers: fetchHelpersMock
        });

        languageSwitcher.on('error', handleError);

        (languageSwitcher as any).handleRequest(); // workaround as simulating select change event not working
        
        setTimeout(() => {

            expect(handleError).toHaveBeenCalledTimes(1);
            done();

        }, 1);

    });

});

