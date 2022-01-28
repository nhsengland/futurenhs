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
        
    const testDialog: any = document.createElement('dialog'); 
    if (testDialog.showModal ) {
        
            document.querySelector(".c-modal").remove();

        } else {
            
            document.querySelector(".c-dialog:not(.c-modal)").remove();

                let safariDialogConfirmed = false;   
                document.addEventListener('click', function (e: any) {
                e = e || window.event;
                const target: any = e.target || e.srcElement; 
                    let modalId:any = null;
            
                    if (target.hasAttribute('data-toggle') && target.getAttribute('data-toggle') == 'c-modal' && safariDialogConfirmed === false) {
                        if (target.hasAttribute('data-target')) {
                            e.preventDefault();
                            const m_ID = target.getAttribute('data-target');
                            document.getElementById(m_ID).classList.add('c-open');
                            modalId = target.getAttribute('data-dialog-id');
                        }
                    }
                
                    // Close modal window with 'data-dismiss' attribute is clicked
                    if ((target.hasAttribute('data-dismiss') && target.getAttribute('data-dismiss') === 'c-modal')) {
                        e.preventDefault();
                        const modal = document.querySelector('[class="c-modal c-dialog c-open"]');
                        modal && modal.classList.remove('c-open');
                        safariDialogConfirmed = false;
                    }

                    if ((target.hasAttribute('data-dismiss') && target.getAttribute('data-dismiss') === 'c-modal-accept')) {
                        
                    e.preventDefault();
                        const modal = document.querySelector('[class="c-modal c-dialog c-open"]');
                        const modalID = modal.id;
                        const modalButtonAction: any  = document.querySelector(`#modal-close[data-target="${modalID}"]`);
                        
                        modal && modal.classList.remove('c-open');
                        safariDialogConfirmed = true;
                        modalButtonAction.click();
                    } 
                
                
                
            
                }, false);
            
     
            }   
   
        }
    


}
