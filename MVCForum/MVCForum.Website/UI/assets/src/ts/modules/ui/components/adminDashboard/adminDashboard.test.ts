import { AdminDashboard } from './index';

const endpointReturnedData = {
    '/Admin/Dashboard/mockApi1': '<span>result1</span>',
    '/Admin/Dashboard/mockApi2': '<span>result2</span>',
    '/Admin/Dashboard/mockApi3': '<span>result3</span>',
};

const jQueryPostMockedFn = jest.fn((endpoint: string, successCallback: Function) => {
    
    const data: string = endpointReturnedData[(endpoint as any)];

    successCallback(data);
    
});

const jQueryHtmlMockedFn = jest.fn();

beforeEach(() => {


    ($.post as any) = jQueryPostMockedFn;

    (jQuery.fn as any).html = jQueryHtmlMockedFn;

    document.body.innerHTML = `
        <div id="main">
          <div class="test1"></div>
            <div class="test2"></div>
            <div class="test3"></div>
        </div>`;

});

describe('Admin Dashboard', () => {

    it('fetches APIs and gets the expected HTML response', () => {

        const classNamesToEndpoints = {
            'test1': 'mockApi1',
            'test2': 'mockApi2',
            'test3': 'mockApi3'
        }

        new AdminDashboard({ fetchTargets: classNamesToEndpoints, wrapperSelector: null }).bindDataToHtmlElements();

        expect(jQueryPostMockedFn).toBeCalledTimes(3);
        expect(jQueryHtmlMockedFn).toBeCalledTimes(3);

        for (const endpoint in endpointReturnedData) {

            if (Object.prototype.hasOwnProperty.call(endpointReturnedData, endpoint)) {
                
                const apiHtmlReturned: string  = endpointReturnedData[endpoint];

                expect(jQueryHtmlMockedFn).toBeCalledWith(apiHtmlReturned);
                
            }
        }

    });



})