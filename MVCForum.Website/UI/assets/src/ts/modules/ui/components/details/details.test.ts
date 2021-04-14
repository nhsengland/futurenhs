import { Details } from './index';

beforeEach(() => {

    document.body.innerHTML = `
        <div id="main">
            <details id="mock">
                <summary data-open-icon="icon-chevron-up" data-closed-icon="icon-chevron-down">
                    More information...
                    <svg>
                        <use xlink:href=""></use>
                    </svg>
                </summary>
                <p>Content</p>
            </details>
        </div>`;

});

describe('Details', () => {

    it('Binds successfully', () => {

        const selector: HTMLDetailsElement = document.querySelector('#mock');
        const details: Details = new Details(selector);

        expect(details).toBeTruthy();

    });

    it('Sets correct icon on init when detail is closed', () => {

        const selector: HTMLDetailsElement = document.querySelector('#mock');
        const details: Details = new Details(selector);
        const xLinkHref: string = selector.querySelector('use').getAttribute('xlink:href');

        expect(xLinkHref).toEqual('#icon-chevron-down');

    });

    it('Sets correct icon on init when detail is open', () => {

        const selector: HTMLDetailsElement = document.querySelector('#mock');

        selector.setAttribute('open', 'true');

        const details: Details = new Details(selector);
        const xLinkHref: string = selector.querySelector('use').getAttribute('xlink:href');

        expect(xLinkHref).toEqual('#icon-chevron-up');

    });

});

