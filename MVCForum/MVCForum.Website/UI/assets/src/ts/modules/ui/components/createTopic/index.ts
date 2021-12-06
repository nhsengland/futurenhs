import { UIComponentBase } from '@modules/ui/componentBase';
import { Toast } from '@modules/ui/components/toast';

interface Config {
    wrapperSelector: HTMLElement;
};

interface Dependencies {
    components: {
        toast: Toast
    }
};

/**
 * CreateTopic 
 */
export class CreateTopic extends UIComponentBase {


    wrapperSelector: HTMLElement = undefined;
    toast: Toast = undefined;

    constructor(config: Config, dependencies: Dependencies) {

        super(config);

        this.wrapperSelector = config.wrapperSelector;
        this.toast = dependencies.components.toast;


        this.polls();
        this.chooseGroup();
        this.getRelatedQuestions();

    }


    getRelatedQuestions = () => {

        const createTopicNameInput = <HTMLInputElement>this.wrapperSelector.getElementsByClassName('createtopicname')?.[0];

        if (!createTopicNameInput) {
            return;
        }      

        const onFocusOut: () => void = () => {

            //remove the whitespaces from the beginning and end 
            const inputValueTrimmed: string = (createTopicNameInput.value).replace(/^\s+|\s+$/g, '');

            if (inputValueTrimmed.length < 4) {
                return;
            }

            $.ajax({
                url: '/Topic/GetSimilarTopics',
                type: 'POST',
                dataType: 'html',
                data: {
                    searchTerm: inputValueTrimmed
                },
                success: (data: string) => {

                    if (!data) {
                        return;
                    }

                    const relatedTopicsKey: Element = this.wrapperSelector.getElementsByClassName('relatedtopicskey')?.[0];
                    const relatedTopicsHolder: Element = this.wrapperSelector.getElementsByClassName('relatedtopicsholder')?.[0];

                    relatedTopicsKey.innerHTML = data;
                    $(relatedTopicsHolder).show();

                },
                error: (xhr, ajaxOptions, thrownError: string) => {
                    this.toast.show("Error: " + xhr.status + " " + thrownError);
                }
            });


        }

        createTopicNameInput.addEventListener('focusout', onFocusOut)



    }

    chooseGroup = () => {

        const chooseGroupDropdown: Element = this.wrapperSelector.getElementsByTagName('select')?.[0];
        const toast: Toast = this.toast;

        if (!chooseGroupDropdown) {
            return;
        }

        const createSticky: HTMLCollectionOf<Element> = this.wrapperSelector.getElementsByClassName('createsticky');
        const createLocked: HTMLCollectionOf<Element> = this.wrapperSelector.getElementsByClassName('createlocked');
        const createUploadFiles: HTMLCollectionOf<Element> = this.wrapperSelector.getElementsByClassName('createuploadfiles');
        const pollCreateButtonHolder: HTMLCollectionOf<Element> = this.wrapperSelector.getElementsByClassName('pollcreatebuttonholder');
        const createTagMessage: HTMLCollectionOf<Element> = this.wrapperSelector.getElementsByClassName('createtagmessage');

        const permissionToElementMap: {
            [key: string]: HTMLCollectionOf<Element>
        } = {
            CanStickyTopic: createSticky,
            CanLockTopic: createLocked,
            CanUploadFiles: createUploadFiles,
            CanCreatePolls: pollCreateButtonHolder,
            CanCreateTags: createTagMessage
        };

        const displayElementsWithPermission: (topicPermissions: {
            [key: string]: boolean
        }) => void = (topicPermissions) => {

            for (const topicPermission in topicPermissions) {

                if (Object.prototype.hasOwnProperty.call(topicPermissions, topicPermission)) {

                    const shouldDisplayElement: boolean = topicPermissions[topicPermission];
                    const topicElements = permissionToElementMap[topicPermission];

                    if (shouldDisplayElement) {

                        $(topicElements).show();

                    } else {

                        $(topicElements).hide();

                    }

                }

            }

        }

        const handleOnChange: (e: Event) => void = function (e: Event) {

            e.preventDefault();

            const selectedValue: string = this.value;

            if (!selectedValue) {
                return;
            }

            $.ajax({
                url: '/Topic/CheckTopicCreatePermissions',
                type: 'POST',
                dataType: 'json',
                data: { catId: selectedValue },
                success: function (topicPermissions) {

                    displayElementsWithPermission(topicPermissions);

                },
                error: (xhr, ajaxOptions, thrownError) => {

                    toast.show("Error: " + xhr.status + " " + thrownError);

                }
            });

        };

        chooseGroupDropdown.addEventListener('change', handleOnChange);
        chooseGroupDropdown.dispatchEvent(new Event('change'));

    }

    polls = () => {

        let currentAnswersInPoll: number = 0;
        const pollAnswerHolder = <Element>this.wrapperSelector.getElementsByClassName('pollanswerholder')?.[0];
        const pollAnswerList = <HTMLUListElement>this.wrapperSelector.getElementsByClassName('pollanswerlist')?.[0];
        const createPollButton = <HTMLAnchorElement>this.wrapperSelector.getElementsByClassName('createpollbutton')?.[0];
        const removePollButton = <HTMLAnchorElement>this.wrapperSelector.getElementsByClassName('removepollbutton')?.[0];
        const addAnswerButton = <HTMLAnchorElement>this.wrapperSelector.getElementsByClassName('addanswer')?.[0];
        const removeAnswerButton = <HTMLAnchorElement>this.wrapperSelector.getElementsByClassName('removeanswer')?.[0];       

        const createPool = (e: Event) => {

            e.preventDefault();           

            $(pollAnswerHolder).show();
            $(createPollButton).hide();
            $(removePollButton).show();

            addAnswer();

        }

        const removePool = (e: Event) => {

            e.preventDefault();

            $(pollAnswerHolder).hide();
            $(removePollButton).hide();
            $(createPollButton).show();

            pollAnswerList.innerHTML = '';

            currentAnswersInPoll = 0;


        }

        const addAnswer = (e?: Event) => {

            e?.preventDefault();

            const placeholder: string = 'Type A Poll Answer Here';
            const newAnswerInput: string = '<input type="text" name="PollAnswers[' + currentAnswersInPoll + '].Answer" id="PollAnswers_' + currentAnswersInPoll + '_Answer" class="form-control" value="" placeholder="' + placeholder + '" />';
            const newAnswerWrapper: Element = document.createElement('li');

            newAnswerWrapper.setAttribute('id', `answer${currentAnswersInPoll}`);
            newAnswerWrapper.innerHTML = newAnswerInput;

            pollAnswerList.appendChild(newAnswerWrapper);

            currentAnswersInPoll = pollAnswerList.childElementCount;

        }

        const removeAnswer = (e: Event) => {

            e.preventDefault();

            const lastAnswer: ChildNode = pollAnswerList.lastChild;

            if (currentAnswersInPoll < 2) {
                return;
            }

            pollAnswerList.removeChild(lastAnswer);

            currentAnswersInPoll = pollAnswerList.childElementCount;

        }

        addAnswerButton?.addEventListener('click', addAnswer);
        removePollButton?.addEventListener('click', removePool);
        createPollButton?.addEventListener('click', createPool);
        removeAnswerButton?.addEventListener('click', removeAnswer);

    }

}
