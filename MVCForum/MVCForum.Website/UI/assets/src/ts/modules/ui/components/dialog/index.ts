import { UIComponentBase } from '@modules/ui/componentBase';
export interface HTMLDialogElement extends HTMLElement {
    open?: boolean,
    returnValue?: string,
    close?: ()=>any,
    show?: ()=>any,
    showModal?: ()=>any
}

declare var HTMLDialogElement: {
    prototype: HTMLDialogElement;
    new(): HTMLDialogElement;
};

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
        const dialogElement: HTMLDialogElement = document.getElementById(dialogId);
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



                
        const supportNode = document.getElementById('support');
        const testDialog = document.createElement('dialog');
       if ( typeof(supportNode) != 'undefined' && supportNode != null) { 
           if (testDialog.showModal ) {
                supportNode.style.color = 'blue';
                supportNode.textContent = 'YES';
                } else {
                supportNode.style.color = 'red';
                supportNode.textContent = 'NO';
            }   
        }
        

    }


}
