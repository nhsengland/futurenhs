import { Input } from './index';

const focusRequirements = 'notEmpty';
const inputId = 'someId';
const holderId = 'holder';
const containerId = 'containerId';

beforeEach(() => {

    document.body.innerHTML = `
        <div id="main">
            <div id="${containerId}" class="js-input-container">
                <div id="${holderId}" class="js-input-holder">
                    
                    <p role="alert" class="js-input-error-notEmpty">Error</p>

                    <input id="${inputId}" class="js-input" data-focus-requirements="${focusRequirements}"></input>
                    
                </div>
            </div>
        </div>`;

});



describe('Input component', () => {

    it('Binds successfully and displays error when validation criteria is not met', () => {
        
        const focusValidators = {
            notEmpty: () => false,
        };

        const holder: Element = document.getElementById(holderId);
        const wrapperSelector: HTMLElement = document.getElementById(containerId);
        const inputElement = <HTMLInputElement>document.getElementById(inputId);
        
        new Input({
            focusValidators: focusValidators,
            wrapperSelector: wrapperSelector,
        });

        inputElement.focus();
        inputElement.blur();

        
        expect(holder.classList.contains('c-form-group--error')).toBeTruthy();
        expect(inputElement.classList.contains('c-input--error')).toBeTruthy();


    });

})


