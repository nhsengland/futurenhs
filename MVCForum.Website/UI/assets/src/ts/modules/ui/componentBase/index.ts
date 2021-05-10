import breakPoints from '../../../../scss/variables/_break-points.scss';
import fetchHelpers from '@utilities/fetch';
const EventEmitter = require('events');


EventEmitter.defaultMaxListeners = 100;

/**
 * Base for use by UI components
 */
export class UIComponentBase extends (EventEmitter as { new(): any; }) {

    public css: {
        breakPoints?: any;
    } = {};

    constructor(config: {
        wrapperSelector: any
    }, dependencies?: {
        fetchHelpers?: typeof fetchHelpers;
        components?: any;
    }) {

        super();

        if (breakPoints?.locals) {

            for (const key in breakPoints.locals) {

                breakPoints.locals[key] = parseInt(breakPoints.locals[key], 10);

            };

            /**
             * Make CSS breakpoints set in SCSS available to components
             */
            this.css.breakPoints = breakPoints.locals;

        }

    }

}
