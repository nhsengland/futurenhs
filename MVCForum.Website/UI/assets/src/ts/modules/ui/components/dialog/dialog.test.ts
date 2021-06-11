import { Dialog } from './index';

let dialogFunctionalitiesMock = undefined;
let triggerDone: jest.Mock= undefined; 

const dialogId = 'someid123';
const dialogTriggerId = 'trigger';
const confirmButtonId = 'confirm';
const cancelButtonId = 'cancel';

beforeEach(() => {

    triggerDone = jest.fn();

    dialogFunctionalitiesMock = {
        close: jest.fn(),
        show: jest.fn(),
    };

    document.body.innerHTML = `
        <div id="main">
            
            <a id="${dialogTriggerId}" class="js-dialog" data-dialog-id="${dialogId}">
                needs dialog confirmation
            </a>

            <dialog id="${dialogId}" class="c-dialog">

                <div class="c-dialog_content">
                    
                    <button id=${cancelButtonId} class="js-dialog-cancel">
                        Cancel
                    </button>
                    <button id=${confirmButtonId} class="js-dialog-confirm">
                        Yes, Log Out
                    </button>

                </div>
            
            </dialog>

        </div>`;

});



describe('Generic dialog', () => {

    it('Cancelling dialog and preventing the default behaviour', () => {

        const dialogTriggerElement = document.getElementById(dialogTriggerId);
        const dialog = <HTMLDialogElement>document.getElementById(dialogId);
        const dialogCancelButton = <HTMLButtonElement>document.getElementById(cancelButtonId);

        dialog.close = dialogFunctionalitiesMock.close; 
        dialog.show = dialogFunctionalitiesMock.show;

        dialogTriggerElement.addEventListener('click', triggerDone);

        new Dialog({
            wrapperSelector: dialogTriggerElement
        });

        dialogTriggerElement.click();
        
        expect(triggerDone).toBeCalledTimes(1);

        dialogCancelButton.click();

        expect(dialogFunctionalitiesMock.close).toBeCalledTimes(1);
        
        expect(triggerDone).toBeCalledTimes(1);

        
    });

    it('Triggers the expected behaviour if the dialog has been confirmed', () => {

        const dialogTriggerElement = document.getElementById(dialogTriggerId);
        const dialog = <HTMLDialogElement>document.getElementById(dialogId);
        const dialogConfirmButton = <HTMLButtonElement>document.getElementById(confirmButtonId);

        dialog.show = dialogFunctionalitiesMock.show; 

        dialogTriggerElement.addEventListener('click', triggerDone);

        new Dialog({
            wrapperSelector: dialogTriggerElement
        });

        dialogTriggerElement.click();
        
        expect(triggerDone).toBeCalledTimes(1);

        dialogConfirmButton.click();

        expect(dialogFunctionalitiesMock.show).toBeCalledTimes(1);
        
        expect(triggerDone).toBeCalledTimes(2);

        
    });


})


