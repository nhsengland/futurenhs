import { UIComponentBase } from '@modules/ui/componentBase';

/**
 * SVG icon dynamic updates helper
 */
export class Icon extends UIComponentBase {

    selector: any = undefined;
    useElement: any = undefined;
    name: string = '';

    constructor(selector: SVGSVGElement, name: string) {

        super();

        this.selector = selector;
        this.name = name;

        this.update = this.update.bind(this);

        return this;

    }

    public update(name: string): any {

        this.name = name;
        this.useElement = this.selector.querySelector('use');
        this.useElement?.setAttribute('xlink:href', `#${this.name}`);

    }

}
