﻿import { UIComponentBase } from '@modules/ui/componentBase';

/**
 * Toast
 */
export class Toast extends UIComponentBase {

    wrapperSelector: HTMLElement = undefined;
    timeOutMillis: number = undefined;
    messageText: string = undefined;
    isShowInProgress: boolean = false;
    tmpShowInProgress: ReturnType<typeof setTimeout> = null;

    constructor(config: {
        wrapperSelector: HTMLElement,
        messageText?: string;
        timeOutMillis?: number;
    }) {

        super(config);

        this.wrapperSelector = config.wrapperSelector;
        this.timeOutMillis = config.timeOutMillis ?? 5000;
        this.messageText = config.messageText ?? undefined;

        this.getMessageMarkup = this.getMessageMarkup.bind(this);
        this.show = this.show.bind(this);

        if (this.messageText) {

            this.show();

        }

        return this;

    }

    private getMessageMarkup = (messageText: string): string => {

        return `<div class="alert alert-info fade in" role="alert">
                    <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                    ${messageText ?? this.messageText}
                </div>`;

    }

    public show = (messageText?: string): void => {

        const messageMarkup: string = this.getMessageMarkup(messageText);

        this.wrapperSelector.innerHTML = messageMarkup;

        $(this.wrapperSelector).show();

        const showMessageText = () => {

            this.isShowInProgress = false;
            
            $(this.wrapperSelector).fadeOut();

            this.wrapperSelector.innerHTML = '';

        }

        if (this.isShowInProgress) {

            clearTimeout(this.tmpShowInProgress);
            
            this.tmpShowInProgress = setTimeout(showMessageText, this.timeOutMillis);

            return;

        }

        this.isShowInProgress = true;

        this.tmpShowInProgress = setTimeout(showMessageText, this.timeOutMillis);

    }

}
