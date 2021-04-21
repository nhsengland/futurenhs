import { UIComponentBase } from './index';

beforeEach(() => {

    // Create a main html element to append test elements to.
    document.body.innerHTML = `
        <div id="main">
            <input type="text"/>
        </div>`;

});

describe('UI Component Base', () => {

    it('Binds successfully', () => {

        const uiComponentBase: UIComponentBase = new UIComponentBase({
            wrapperSelector: document.getElementById('main')
        });

        expect(uiComponentBase).toBeTruthy();

    });

});

