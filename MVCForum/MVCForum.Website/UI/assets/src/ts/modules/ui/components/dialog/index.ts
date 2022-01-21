import { UIComponentBase } from '@modules/ui/componentBase';

/**
 * Dialog
 */
export class Dialog extends UIComponentBase {

    wrapperSelector: HTMLElement = undefined;

    constructor(config: {
        wrapperSelector: HTMLElement;
    }) {

        super(config);

        this.wrapperSelector = config.wrapperSelector;

        this.init();

        this.openInit();

        return this;

    }

    public init = (): void => {

        let isDialogConfirmed: boolean = false;         

        const dialogId: string = this.wrapperSelector.getAttribute('data-dialog-id');
        const dialogElement = <HTMLDialogElement>document.getElementById(dialogId);
        const cancelButton: HTMLButtonElement = dialogElement.querySelector('.js-dialog-cancel');
        const confirmButton: HTMLButtonElement = dialogElement.querySelector('.js-dialog-confirm');

        cancelButton.addEventListener('click', (e: Event) => {

            e.preventDefault();

            dialogElement.close();

        });

        confirmButton.addEventListener('click', (e: Event) => {

            e.preventDefault();

            isDialogConfirmed = true;

            this.wrapperSelector.click();

        });

        this.wrapperSelector.addEventListener('click', (e: Event) => {
            
            if(!isDialogConfirmed){

                e.preventDefault();

                dialogElement.show();

            };

            isDialogConfirmed = false;

        });

       

    }


    public openInit = () => {

        const dialogElement = document.getElementsByTagName('dialog');

        const openCheck = (dialog) => {
            if(dialog.open) {
               console.log('Dialog open');
                dialog.setAttribute('open', 'false');   
                dialog.removeAttribute('open')
                dialog.close();
            } else {
               console.log('Dialog closed');
            }
        }

        openCheck(dialogElement)
        

    }


}
