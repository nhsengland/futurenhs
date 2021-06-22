import { LoadMoreButton } from './index';

let fetchHelpersMock = undefined;

const mockAppendTargetId = 'postList';
const requestId = 'someid123';
const requestIndex = 1;
const loadMoreButtonId = 'loadMoreId';
const mockHtmlResponse  = '<div>some post</div>';

beforeEach(() => {

    fetchHelpersMock = {
        setFetchOptions: jest.fn(),
        fetchData: jest.fn(() => Promise.resolve(mockHtmlResponse)),
        fetchWithTimeOut: jest.fn(),
        getErrorMessageString: jest.fn(() => 'Error')    
    }

    document.body.innerHTML = `
        <div id="main">
            
            <div id="${mockAppendTargetId}"></div>

            <button id="${loadMoreButtonId}"
                    data-target-id="${mockAppendTargetId}"
                    data-request-id="${requestId}"
                    data-endpoint-type="getPostComments"
                    data-maxim-requests="2"
                    data-request-index="${requestIndex}"
                    class="c-button c-button--secondary js-loadmore u-hidden">
                Load more
            </button>

        </div>`;

});



describe('Load more button', () => {

    it('When JS is enabled displays the button by removing the hidden utility class', () => {

        const appendTargetElement = document.getElementById(mockAppendTargetId); 
        const buttonElement = <HTMLButtonElement>document.getElementById(loadMoreButtonId);
        
        new LoadMoreButton({
            getFetchUrl: ()=>'',
            requestIndex: 1,
            maximRequests: 2,
            appendTargetElement: appendTargetElement,
            wrapperSelector: buttonElement
        }, {
            fetchHelpers: fetchHelpersMock
        });

        expect(buttonElement.classList.contains('u-hidden')).toBeFalsy();
        
    });

    it('Hides the button when there are no more posts to fetch', () => {

        const appendTargetElement = document.getElementById(mockAppendTargetId); 
        const buttonElement = <HTMLButtonElement>document.getElementById(loadMoreButtonId);
        buttonElement.setAttribute('data-maxim-requests','1');

        new LoadMoreButton({
            getFetchUrl: ()=>'',
            requestIndex: 2,
            maximRequests: 1,
            appendTargetElement: appendTargetElement,
            wrapperSelector: buttonElement
        }, {
            fetchHelpers: fetchHelpersMock
        });

        expect(buttonElement.classList.contains('u-hidden')).toBeTruthy();

    });

    it('Appends HTML response to target', (done) => {

        const appendTargetElement: HTMLElement = document.getElementById(mockAppendTargetId);
        const buttonElement = <HTMLButtonElement>document.getElementById(loadMoreButtonId);
        
        new LoadMoreButton({
            getFetchUrl: ()=>'',
            requestIndex: 1,
            maximRequests: 2,
            appendTargetElement: appendTargetElement,
            wrapperSelector: buttonElement
        }, {
            fetchHelpers: fetchHelpersMock
        });

        expect(appendTargetElement.innerHTML.length).toBe(0);

        buttonElement.click();

        setTimeout(() => {

            expect(appendTargetElement.innerHTML.length).toBe(mockHtmlResponse.length);

            done();

        }, 500);
        
    });


    it('Fetch data from given url', (done) => {

        const testUrl = 'example.com/query';
        const appendTargetElement: HTMLElement = document.getElementById(mockAppendTargetId);
        const buttonElement = <HTMLButtonElement>document.getElementById(loadMoreButtonId);
        
        new LoadMoreButton({
            getFetchUrl: () => testUrl,
            requestIndex: 1,
            maximRequests: 2,
            appendTargetElement: appendTargetElement,
            wrapperSelector: buttonElement
        }, {
            fetchHelpers: fetchHelpersMock
        });

        buttonElement.click();

        setTimeout(() => {

            expect(fetchHelpersMock.fetchData).toBeCalledWith(testUrl, expect.anything(),expect.anything());

            done();

        }, 500);
        
    });

})


