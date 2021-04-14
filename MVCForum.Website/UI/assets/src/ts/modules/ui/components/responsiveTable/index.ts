import { UIComponentBase } from '@modules/ui/componentBase/index';

/**
 * Responsive table
 */
export class ResponsiveTable extends UIComponentBase {

    wrapperSelector: HTMLTableElement = undefined;

    constructor(config: {
        wrapperSelector: HTMLTableElement
    }) {

        super();

        this.wrapperSelector = config.wrapperSelector;
        
        if (this.wrapperSelector) {
            
            const headerText: Array<string> = [];
            const headers: Array<HTMLElement> = Array.from(this.wrapperSelector.querySelectorAll('th'));
            const tableBody: HTMLElement = this.wrapperSelector.querySelector('tbody');

            headers.forEach((header: HTMLElement) => {

                headerText.push(header.textContent.replace(/\r?\n|\r/, ""));

            });

            if ((tableBody as any).rows != null) {

                for (var i = 0, row; row = (tableBody as any).rows[i]; i++) {

                    for (var j = 0, col; col = row.cells[j]; j++) {

                        col.setAttribute("data-th", headerText[j]);

                    }

                }

            }

        }

    }

}
