import { CreateTopic } from './index';

const jQueryAjaxMockedFn = jest.fn();
const jQueryHide = jest.fn();
const jQueryShow = jest.fn();

beforeEach(() => {

    ($.ajax as any) = jQueryAjaxMockedFn;
    
    (jQuery.fn as any).show = jQueryShow;
    (jQuery.fn as any).hide = jQueryHide;

    document.body.innerHTML = `
        <div id="main">
        
            <input id="createTopicInputTest" class="createtopicname" type="text" value="someValue">

            <div class="relatedtopicsholder">
                <h5>Questions that may already have your answer</h5>
                <div class="relatedtopicskey"></div>
            </div>

            <select>
                <option value></option>
                <option value="selectValue1">Example 1</option>
                <option value="selectValue2">Example 2</option>
                <option value="selectValue3">Example 3</option>
            </select>
            
            <div class="createsticky"></div>
            <div class="createlocked"></div>
            <div class="createuploadfiles"></div>
            <div class="pollcreatebuttonholder"></div>
            <div class="createtagmessage"></div>

            <a class="createpollbutton"></a>
            <a class="removepollbutton"></a>
            <a class="addanswer"></a>
            <a class="removeanswer"></a>

            <div class="pollanswerholder">
                <ul class="pollanswerlist"></ul>
            </div>
            
        </div>`;

});

describe('Create topic', () => {

    it("Get related questions - on input foucs out fetch data", () => {

        const main: HTMLElement = document.getElementById('main');

        new CreateTopic({
            wrapperSelector: main
        }, {
            components: {
                toast: null
            }
        });

        const topicInput = document.getElementById('createTopicInputTest');
        const focusOutEvent = new Event('focusout');

        topicInput.dispatchEvent(focusOutEvent);
        topicInput.dispatchEvent(focusOutEvent);
        topicInput.dispatchEvent(focusOutEvent);

        expect(jQueryAjaxMockedFn).toBeCalledTimes(3);

    });


    it("Choose group - show/hide elements based on selected group permission", () => {

        const newValue = 'selectValue1';

        const permissionsData = {
            CanStickyTopic: true,
            CanLockTopic: true,
            CanUploadFiles: true,
            CanCreatePolls: false,
            CanCreateTags: false,
        }

        const localjQueryAjaxMockedFn = jest.fn(({ data: { catId }, success }) => {

            expect(catId).toBe(newValue);

            success(permissionsData);

        });

        ($.ajax as any) = localjQueryAjaxMockedFn;

        const main: HTMLElement = document.getElementById('main');

        new CreateTopic({
            wrapperSelector: main
        }, {
            components: {
                toast: null
            }
        });

        const selectElement = <HTMLSelectElement>document.getElementsByTagName('select')[0];
        const changeEvent = new Event('change');

        selectElement.selectedIndex = 1;
        selectElement.dispatchEvent(changeEvent);

        expect(selectElement.value).toBe(newValue);
        expect(localjQueryAjaxMockedFn).toBeCalledTimes(1);

        expect(jQueryShow).toBeCalledTimes(3);
        expect(jQueryHide).toBeCalledTimes(2);

    });


    it("Polls - create/remove poll, add/remove answers to created poll", () => {

        const main: HTMLElement = document.getElementById('main');

        new CreateTopic({
            wrapperSelector: main
        }, {
            components: {
                toast: null
            }
        });

        const createPollButton = <HTMLAnchorElement>document.getElementsByClassName('createpollbutton')?.[0];
        const removePollButton = <HTMLAnchorElement>document.getElementsByClassName('removepollbutton')?.[0];
        const addAnswerButton = <HTMLAnchorElement>document.getElementsByClassName('addanswer')?.[0];
        const removeAnswerButton = <HTMLAnchorElement>document.getElementsByClassName('removeanswer')?.[0];
        const pollAnswerList = <HTMLUListElement>document.getElementsByClassName('pollanswerlist')?.[0];

        expect(pollAnswerList.childElementCount).toEqual(0);

        createPollButton.click();

        expect(jQueryShow).toBeCalledTimes(2);
        expect(jQueryHide).toBeCalledTimes(1);
        expect(pollAnswerList.childElementCount).toEqual(1);

        addAnswerButton.click();
        addAnswerButton.click();

        expect(pollAnswerList.childElementCount).toEqual(3);
        
        removeAnswerButton.click();
        
        expect(pollAnswerList.childElementCount).toEqual(2);

        removePollButton.click();
        
        expect(jQueryShow).toBeCalledTimes(3);
        expect(jQueryHide).toBeCalledTimes(3);
        expect(pollAnswerList.childElementCount).toEqual(0);
        
    });
})