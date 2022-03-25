import { TopicPost } from './index';
import { Toast } from '@modules/ui/components/toast';
import * as fetchHelpers from '@utilities/fetch';

const jQueryHide = jest.fn();
const jQueryShow = jest.fn();
const jQueryAjaxMockedFn = jest.fn();
const jQueryToggle = jest.fn();
const jQueryFadeOut = jest.fn();

beforeEach(() => {

    ($.ajax as any) = jQueryAjaxMockedFn;

    (jQuery.fn as any).show = jQueryShow;
    (jQuery.fn as any).hide = jQueryHide;
    (jQuery.fn as any).toggle = jQueryToggle;
    (jQuery.fn as any).fadeOut = jQueryFadeOut;

    document.body.innerHTML = `
        <div id="main">

            <a class="pollvotebutton"></a>
            <div class="pollcontainer">
                <input id="pollAnswerSelect" type="radio" class="pollanswerselect" data-answerid="answer-id-123" />
                <div class="pollvotebuttonholder"></div>
                <input id="selectedPollAnswer" class="selectedpollanswer" />
            </div>
            <input id="Poll_Id" value="id-poll-123">
            <input class="selectedpollanswer" value="answer-A">


            <div class="postlikedby">

                <li id="allLikesListItem" class="othersliked" data-postid="post-123">
                    <a id="getAllLikes" href="#"></a>
                </li>

            </div>

            <div class="post">
            
                <a id="postshowattach" class="postshowattach"></a>
                <div class="postuploadholder">
                    <input id="postUploadButton" type="submit" class="postuploadbutton">
                    <span class="ajaxspinner"></span>
                </div>
                
                
            </div>

            <div class="postadmin">
                <button id="postOptions" class="postoptions"></button>
                <ul class="postadminlist">
                </ul>
            </div>

            <a id="showMorePosts" class="showmoreposts">
                <span class="smpactive"></span>
                <span class="smploading"></span>
            </a>

            <input id="pageIndex" value="1" />
            <input id="topicId" value="topic-123" />
            <input id="totalPages" value="2" />


            
        </div>`;

});

describe('Topic post', () => {

    it("Vote in poll", () => {

        const localjQueryAjaxMockedFn = jest.fn(({ data: data, success }) => {

            const updatePollViewModel = JSON.parse(data);

            const { PollId, AnswerId } = updatePollViewModel;

            const result = `voted: ${PollId} ${AnswerId}`;

            success(result);

        });

        ($.ajax as any) = localjQueryAjaxMockedFn;

        const main: HTMLElement = document.getElementById('main');

        new TopicPost({
            wrapperSelector: main
        }, {
            fetchHelpers: fetchHelpers,
            components: {
                toast: new Toast({
                    wrapperSelector: null
                })
            }
        });

        const pollVoteButton = <HTMLAnchorElement>document.getElementsByClassName('pollvotebutton')?.[0];

        pollVoteButton.click();

        const pollContainer = document.getElementsByClassName('pollcontainer')[0];

        //expect(pollContainer.innerHTML).toBe('voted: id-poll-123 ');

    });


    it("Get all likes", () => {

        const localjQueryAjaxMockedFn = jest.fn(({ data: { id }, success }) => {

            success(`id: ${id}`);

        });

        ($.ajax as any) = localjQueryAjaxMockedFn;

        const main: HTMLElement = document.getElementById('main');

        new TopicPost({
            wrapperSelector: main
        }, {
            fetchHelpers: fetchHelpers,
            components: {
                toast: new Toast({
                    wrapperSelector: null
                })
            }
        });

        const allLikesAnchor = <HTMLAnchorElement>document.getElementById('getAllLikes');

        allLikesAnchor.click();

        const allLikesListItem = <HTMLLIElement>document.getElementById('allLikesListItem');

        //expect(allLikesListItem.innerHTML).toBe('id: post-123');
        
    });


    it("Display file uploader", () => {

        const main: HTMLElement = document.getElementById('main');

        new TopicPost({
            wrapperSelector: main
        }, {
            fetchHelpers: fetchHelpers,
            components: {
                toast: new Toast({
                    wrapperSelector: null
                })
            }
        });

        const displayUploaderAnchor = <HTMLAnchorElement>document.getElementById('postshowattach');

        //show
        displayUploaderAnchor.click();

        //hide
        displayUploaderAnchor.click();

        //show
        displayUploaderAnchor.click();
        
        expect(jQueryToggle).toBeCalledTimes(3);
        
    });

    it("Show post options", () => {

        const main: HTMLElement = document.getElementById('main');

        new TopicPost({
            wrapperSelector: main
        }, {
            fetchHelpers: fetchHelpers,
            components: {
                toast: new Toast({
                    wrapperSelector: null
                })
            }
        });

        const postOptions = <HTMLButtonElement>document.getElementById('postOptions');

        //show
        postOptions.click();

        //hide
        postOptions.click();
        
        expect(jQueryToggle).toBeCalledTimes(2);

    });

    it("Select poll answer", () => {

        const main: HTMLElement = document.getElementById('main');

        new TopicPost({
            wrapperSelector: main
        }, {
            fetchHelpers: fetchHelpers,
            components: {
                toast: new Toast({
                    wrapperSelector: null
                })
            }
        });

        const pollAnswerSelect = <HTMLAnchorElement>document.getElementById('pollAnswerSelect');
        const selectedPollAnswer = <HTMLInputElement>document.getElementById('selectedPollAnswer');     

        //show
        pollAnswerSelect.click();
        
        expect(jQueryShow).toBeCalledTimes(1);
        expect(selectedPollAnswer.value).toBe('answer-id-123');

    });


    it("Display loading attachments", () => {

        const main: HTMLElement = document.getElementById('main');

        new TopicPost({
            wrapperSelector: main
        }, {
            fetchHelpers: fetchHelpers,
            components: {
                toast: new Toast({
                    wrapperSelector: null
                })
            }
        });

        const postUploadButton = <HTMLAnchorElement>document.getElementById('postUploadButton');  

        postUploadButton.click();
        
        expect(jQueryShow).toBeCalledTimes(1);
        expect(jQueryFadeOut).toBeCalledWith('fast');

    });


})