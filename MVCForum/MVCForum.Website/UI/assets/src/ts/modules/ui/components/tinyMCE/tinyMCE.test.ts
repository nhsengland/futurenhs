import { TinyMCE } from './index';

const requirements = 'notEmpty';
const tinyMCEid = 'someId';
const holderId = 'holder';
const submitId = 'submitId';
const clearId = 'clearId';
const containerId = 'containerId';
const setContent = jest.fn();

let tinyMceAPI;

beforeEach(() => {

    tinyMceAPI = {
        get: jest.fn(),
        getContent: jest.fn(),
        setContent: setContent,
    }

    document.body.innerHTML = `
        <div id="main">
            <div id="${containerId}" class="js-tinyMCE-container">
                <div id="${holderId}" class="js-tinyMCE-holder">
                    
                    <p role="alert" class="js-tinyMCE-error-notEmpty">Please provide a comment.</p>

                    <textarea id="${tinyMCEid}" class="js-tinyMCE"></textarea>

                    <button id="${clearId}" class="js-tinyMCE-clear">Clear</button>
                    <button id="${submitId}" class="js-tinyMCE-submit" data-mce-requirements="${requirements}">Submit</button>
                
                    
                </div>
            </div>
        </div>`;

});



describe('TinyMCE', () => {

    it('Binds successfully and displays error when validation criteria is not met', () => {
        
        const validators = {
            notEmpty: () => false,
        };
        
        const submitBtn = <HTMLButtonElement>document.getElementById(submitId);
        const wrapperSelector: HTMLElement = document.getElementById(containerId);

        new TinyMCE({
            validators: validators,
            tinyMceAPI: tinyMceAPI,
            wrapperSelector: wrapperSelector,
        });

        submitBtn.click();

        expect(document.getElementById(holderId).classList.contains('u-has-error')).toBeTruthy();


    });

    it('Test clear button', () => {
        
        const validators = { };
        
        const clearBtn = <HTMLButtonElement>document.getElementById(clearId);
        const wrapperSelector: HTMLElement = document.getElementById(containerId);

        new TinyMCE({
            validators: validators,
            tinyMceAPI: tinyMceAPI,
            wrapperSelector: wrapperSelector,
        });

        clearBtn.click();

        expect(setContent).toBeCalledTimes(1);
        expect(setContent).toBeCalledWith('');

    });



})


