import { UIComponentBase } from '@modules/ui/componentBase';

export interface TagsInputAPIInterface {
    minChars?: number;
    maxChars?: number; // if not provided there is no limit
    removeWithBackspace?: boolean;
    autocomplete_url?: string;
}


interface Config {
    wrapperSelector: HTMLElement;
    tagsInputAdditionalConfig?: TagsInputAPIInterface;
}

/**
 * TagsInput
 */
export class TagsInput extends UIComponentBase {

    tagsInput: any;
    wrapperSelector: HTMLElement = undefined;
    tagsInputAdditionalConfig: TagsInputAPIInterface = {};

    constructor(config: Config) {

        super(config);

        this.wrapperSelector = config.wrapperSelector;
        this.tagsInputAdditionalConfig = config.tagsInputAdditionalConfig;

        this.init();

    }

    private defaultOnAddTag = (tag: string): void => {

        const trimmedTag: string = tag.replace(/\s+/g, '-').toLowerCase();
        const hasWhiteSpace: boolean = /\s/.test(tag);

        if (hasWhiteSpace) {

            this.tagsInput.removeTag(tag);

            if (!this.tagsInput.tagExist(trimmedTag)) {
                this.tagsInput.addTag(trimmedTag);
            }

        }

    }

    public init = (): void => {

        this.tagsInput = (<any>$(this.wrapperSelector)).tagsInput({
            onAddTag: this.defaultOnAddTag,
            ...this.tagsInputAdditionalConfig,
        });

        /**
         * tagsInput plugin hides textarea wrapper element and generates an input sibling
         * the textarea is used to store tag values added in the generated input
         * on input's foucsout event, if the input has a value, it will automatically add the value to textarea by generating an 'Enter' keyboard press
         */
        const generatedInput: Element = (<any>this.wrapperSelector.nextSibling)?.getElementsByTagName('input')[0];

        generatedInput?.addEventListener('focusout', () => {

            const keyboardEnterEvent = $.Event("keypress", { which: 13 });
            $(generatedInput).trigger(keyboardEnterEvent);

        });


    }

}
