import { UIComponentBase } from '@modules/ui/componentBase';

/**
 * Upload button
 */
export class UploadButton extends UIComponentBase {

    wrapperSelector: HTMLElement = undefined;

    constructor(config: {
        wrapperSelector: HTMLElement
    }) {

        super();

        this.wrapperSelector = config.wrapperSelector;

        // TODO: REFACTOR
        $(document).on("change", ".btn-file :file", () => {

            const input: any = $(this);
            const numFiles: number = (input.get(0) as any)?.files.length ?? 1;
            const label: string = (input.val() as string)
                                    .replace(/\\/g, '/')
                                    .replace(/.*\//, '');
            
            input.trigger("fileselect", [numFiles, label]);
        
        });

        // TODO: REFACTOR
        $(".btn-file :file").on("fileselect", (event, numFiles, label) => {

            const input: JQuery = $(this).parents(".input-group").find(":text");
            const log: string = numFiles > 1 ? numFiles + " files selected" : label;

            if (input.length) {
                
                input.val(log);
            
            } else {
                
                if (log) alert(log);

            }

        });

        return this;

    }

}
