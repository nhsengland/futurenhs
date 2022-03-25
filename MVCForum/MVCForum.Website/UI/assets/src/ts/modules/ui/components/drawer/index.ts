import { UIComponentBase } from '@modules/ui/componentBase';
import { lockPageScroll } from '@utilities/css';
import { cssClasses } from '@constants/index';

/**
 * Drawer
 */
export class Drawer extends UIComponentBase {

    wrapperSelector: HTMLElement = undefined;
    loaderMarkup: string = undefined;
    text: any = {};

    constructor(config: {
        wrapperSelector: HTMLElement;
        loaderMarkup: string;
        text: {
            closeConfirmText: string;
        }
    }) {

        super(config);

        this.wrapperSelector = config.wrapperSelector;
        this.loaderMarkup = config.loaderMarkup ?? '<div class="loaderholder"><img src="" + "TODO:largeSpinnerBlockImage" + "" alt=""/></div>';
        this.text = {
            closeConfirmText: config.text.closeConfirmText
        };

        this.setContent = this.setContent.bind(this);
        this.toggle = this.toggle.bind(this);
        this.show = this.show.bind(this);
        this.clear = this.clear.bind(this);
        this.hide = this.hide.bind(this);

        this.toggle();

        return this;

    }

    private setContent = (headingText: string, contentHtml: string): void => {

        const headingElement: HTMLHeadingElement = this.wrapperSelector.querySelector('.cd-panel-header h6');
        const contentElement: HTMLElement = this.wrapperSelector.querySelector('.cd-panel-content');

        headingElement.innerText = headingText;
        contentElement.innerHTML = contentHtml;
    
    }

    private toggle = (): void => {

        this.wrapperSelector.addEventListener('click', (event) => {

            if ($(event.target).is('.cd-panel') || $(event.target).is('.cd-panel-close')) {

                if (typeof window.tinyMCE != 'undefined') {
                    
                    const activeText: string = window.tinyMCE.activeEditor?.getContent?.();
                    
                    if (activeText != '') {
                        
                        if (confirm(this.text.closeConfirmText)) {
                            
                            this.clear();
                            this.close();

                        }

                    } else {

                        this.clear();
                        this.close();

                    }

                } else {

                    this.clear();
                    this.close();

                }

                return false;

            }

        });

    }

    public show = (headingText: string, bodyHtml: string): void => {

        lockPageScroll(true); 

        this.setContent(headingText, bodyHtml);
        this.wrapperSelector.classList.add(cssClasses.IS_VISIBLE);

    }

    public clear = (): void => {

        this.setContent('', this.loaderMarkup);

        window.onbeforeunload = function () { }; //remove onbeforeunload registered by TinyMCE

    }

    public hide = (): void => {

        lockPageScroll(false); 

        this.wrapperSelector.classList.remove(cssClasses.IS_VISIBLE);

    }

}
