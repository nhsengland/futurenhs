import { UIComponentBase } from '@modules/ui/componentBase';

/**
 * SVG icon dynamic updates helper
 */
export class Icon extends UIComponentBase {

    wrapperSelector: any = undefined;
    useElement: any = undefined;
    name: string = '';

    constructor(config: {
        wrapperSelector: SVGSVGElement, 
        name: string
    }) {

        super(config);

        this.wrapperSelector = config.wrapperSelector;
        this.name = config.name;

        this.update = this.update.bind(this);

        return this;

    }

    public update(name: string): any {

        this.name = name;
        this.useElement = this.wrapperSelector.querySelector('use');
        this.useElement?.setAttribute('xlink:href', `#${this.name}`);

    }

}
