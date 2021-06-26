import { UIComponentBase } from '@modules/ui/componentBase';
import { Toast } from '@modules/ui/components/toast';
import * as fetchHelpers from '@utilities/fetch';
import { FetchOptions } from '@appTypes/fetch';

interface Config {
    pollId?: string;
    wrapperSelector: HTMLElement;
    fancyBoxTargetTypes?: Array<string>;
};

interface Dependencies {
    fetchHelpers: typeof fetchHelpers;
    components: {
        toast: Toast
    }
};

/**
 * TopicPost 
 */
export class TopicPost extends UIComponentBase {

    toast: Toast = undefined;
    pollId: string = undefined;
    fancyBoxTargetTypes?: Array<string> = [];
    wrapperSelector: HTMLElement = undefined;
    fetchHelpers: typeof fetchHelpers = undefined;

    constructor(config: Config, dependencies: Dependencies) {

        super(config, dependencies);

        this.pollId = config.pollId; 
        this.fancyBoxTargetTypes = config.fancyBoxTargetTypes;
        this.wrapperSelector = config.wrapperSelector;
        this.toast = dependencies.components.toast;
        this.fetchHelpers = dependencies.fetchHelpers;

        this.init();
    }


    init = (): void => {

        this.votePost();
        this.voteInPoll();
        this.getAllLikes();
        this.favouritePost();
        this.showPostOptions();
        this.selectPollAnswer();
        this.markPostAsSolution();
        this.displayFileUploader();
        this.displayLoadingAttachments();
        this.bindModerateForumEvents('post');
        this.bindModerateForumEvents('topic');

        /* 

        functions were refactored but not ready for use
        the BE serves different files and html strucutre than expected 

        this.initFancyBox();

        */
    }

    public bindFeaturesToPost = (): void =>  {

        this.votePost();
        this.favouritePost();
        this.showPostOptions();
        this.markPostAsSolution();
        this.displayFileUploader();
        this.displayLoadingAttachments();

    }


    voteInPoll = (): void => {

        const wrapperSelector = this.wrapperSelector;
        const pollId = this.pollId;
        const pollVoteButton = <HTMLAnchorElement>wrapperSelector.getElementsByClassName('pollvotebutton')?.[0];
        const handleVoteInPoll = (e: Event): void => {

            e.preventDefault();
            e.stopImmediatePropagation();

            const answerId: string = (<HTMLInputElement>wrapperSelector.getElementsByClassName('selectedpollanswer')?.[0])?.value;

            const updatePollViewModel = {
                PollId: pollId,
                AnswerId: answerId
            };

            const { setFetchOptions, fetchData } = this.fetchHelpers;
            const fetchOptions: FetchOptions = setFetchOptions({
                method: 'POST',
                contentType: 'application/json; charset=UTF-8',
                body: updatePollViewModel,
                customHeaders: {
                    'Accept': 'text/html, */*; q=0.01',
                    'X-Requested-With': 'XMLHttpRequest',
                    'cache-control':'no-cache',
                }
            });

            fetchData({
                url: "/Poll/UpdatePoll",
                options: fetchOptions,
                timeOut: 60000,
                dataType: 'html'
            })
                .then((data: string) => {

                    const pollContainer = wrapperSelector.getElementsByClassName('pollcontainer')[0];
                    pollContainer.innerHTML = data;

                })
                .catch((error: any) => {

                    this.toast.show(`Error: ${error}`);

                });

        }

        pollVoteButton?.addEventListener('click', handleVoteInPoll);

    }

    getAllLikes = (): void => {

        const wrapperSelector = this.wrapperSelector;
        const likedByContainers: Array<Element> = Array.from(wrapperSelector.getElementsByClassName("postlikedby"));

        likedByContainers.forEach((likedByContainer: Element) => {

            const othersLikedItem = <HTMLLIElement>likedByContainer.getElementsByClassName("othersliked")[0];

            if (!othersLikedItem) {
                return;
            }

            const othersLikedItemLink = <HTMLAnchorElement>othersLikedItem.getElementsByTagName('a')[0];
            const id: string = othersLikedItem.getAttribute('data-postid');

            const handleOnClick = (e: Event) => {

                e.preventDefault();

                const { setFetchOptions, fetchData } = this.fetchHelpers;
                const fetchOptions: FetchOptions = setFetchOptions({
                    method: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    body: {
                        id: id
                    },
                    customHeaders: {
                        'Accept': 'text/html, */*; q=0.01',
                        'X-Requested-With': 'XMLHttpRequest',
                        'cache-control':'no-cache',
                    }
                });

                fetchData({
                    url: "/Post/GetAllPostLikes",
                    options: fetchOptions,
                    timeOut: 60000,
                    dataType: 'html'
                })
                    .then((data: string) => {

                        othersLikedItem.innerHTML = data;

                    })
                    .catch((error: any) => {

                        this.toast.show(`Error: ${error}`);

                    });

            }

            othersLikedItemLink.addEventListener('click', handleOnClick);


        })


    }

    displayFileUploader = (): void => {

        const wrapperSelector = this.wrapperSelector;
        const posts: Array<Element> = Array.from(wrapperSelector.getElementsByClassName('post'));

        posts?.forEach((post: Element) => {
            
            const showAttachedContainers: Element = post.getElementsByClassName('postshowattach')?.[0];
            const uploadHolder: Element = post.getElementsByClassName('postuploadholder')[0];

            const handleOnClick = function (e: Event): void {

                e.preventDefault();
                e.stopImmediatePropagation();
    
                $(uploadHolder).toggle();
                
    
            }
            
            showAttachedContainers?.addEventListener('click', handleOnClick);

        });


    }

    showPostOptions = (): void => {

        const wrapperSelector = this.wrapperSelector;
        
        const adminPosts: Array<Element> = Array.from(wrapperSelector.getElementsByClassName('postadmin'));
      
        adminPosts?.forEach((adminPost) => {

            const postOptions: Element = adminPost.getElementsByClassName('postoptions')[0];
            const postAdminList: Element = adminPost.getElementsByClassName('postadminlist')[0];

            const handleOnClick = function (e: Event): void {

                e.preventDefault();
                e.stopImmediatePropagation();
    
                $(postAdminList).toggle();
    
            }

            postOptions.addEventListener('click', handleOnClick);

        });

    }

    selectPollAnswer = (): void => {

        const wrapperSelector = this.wrapperSelector;
        const pollContainer: Element = wrapperSelector.getElementsByClassName('pollcontainer')[0];
        const pollAnswerRadios = Array.from(pollContainer?.getElementsByClassName('pollanswerselect') || []);

        const handleOnClick = function () {

            const id = (this as HTMLInputElement).getAttribute('data-answerid');
            const pollVoteButtonWrapper = <Element>pollContainer.getElementsByClassName('pollvotebuttonholder')[0];
            const selectedPollAnswerInput = <HTMLInputElement>pollContainer.getElementsByClassName('selectedpollanswer')[0];

            selectedPollAnswerInput.value = id;
            $(pollVoteButtonWrapper).show();

        }

        pollAnswerRadios?.forEach((pollAnswerRadio: HTMLInputElement) => {

            pollAnswerRadio.addEventListener('click', handleOnClick);

        });
    }

    initFancyBox = (): void => {

        const fileTypes: Array<string> = this.fancyBoxTargetTypes;
        let files = [];

        fileTypes.forEach((fileType) => {

            const wrapperSelector = this.wrapperSelector;
            const fileQueryDivs: string = `div.fileupload a[href$=\".${fileType}\"]`;
            const fileQueryAnchors: string = `a.fileupload[href$=\".${fileType}\"]`;

            const queryDivFiles = Array.from(wrapperSelector.querySelectorAll(fileQueryDivs));
            const queryAnchorFiles = Array.from(wrapperSelector.querySelectorAll(fileQueryAnchors));

            files = [...files, ...queryDivFiles, ...queryAnchorFiles];

        });

        (<any>$(files))?.fancybox({
            openEffect: "elastic",
            closeEffect: "elastic"
        });

    };

    displayLoadingAttachments = (): void => {

        const wrapperSelector = this.wrapperSelector;
        const uploadContainer = Array.from(wrapperSelector.getElementsByClassName('postuploadholder'));

        uploadContainer.forEach(uploadContainer => {

            const postUploadButton = <HTMLInputElement>uploadContainer.getElementsByClassName('postuploadbutton')[0];
            const loadingMessage = uploadContainer.getElementsByClassName('ajaxspinner')[0];

            postUploadButton.addEventListener('click', function () {

                $(this).fadeOut("fast");
                $(loadingMessage).show();

            })

        });

    }

    markPostAsSolution = (): void => {

        const toast = this.toast;
        const wrapperSelector = this.wrapperSelector;
        const { setFetchOptions, fetchData } = this.fetchHelpers;
        const postSocialContainers: Array<Element> = Array.from(wrapperSelector.getElementsByClassName('postsocial'));
        const allSolutionLinks: Array<Element> = Array.from(wrapperSelector.getElementsByClassName('solutionlink'));
        const allSolutionIcons: Array<Element> = Array.from(wrapperSelector.getElementsByClassName('solution-icon'));

        postSocialContainers.forEach((socialContainer: HTMLAnchorElement) => {

            const solutionLink = <HTMLAnchorElement>socialContainer.getElementsByClassName('solutionlink')?.[0];

            if (!solutionLink) {
                return;
            }

            const solutionIcon = <SVGElement>socialContainer.getElementsByClassName('solution-icon')[0];

            const solutionMarkedUI = function () {

                solutionLink.removeEventListener('click', markSolutionRequest);

                allSolutionLinks.forEach((solutionLink: HTMLAnchorElement) => solutionLink.classList.add('u-visually-hidden'));
                allSolutionIcons.forEach((solutionIcon: SVGElement) => solutionIcon.classList.add('u-visually-hidden'));

                solutionIcon.classList.remove('u-visually-hidden');
                solutionIcon.classList.add('c-ui-icon--green');


            }

            const markSolutionRequest = function (e: Event) {

                e.preventDefault();

                const id: string = this.getAttribute('data-id');

                const fetchOptions: FetchOptions = setFetchOptions({
                    method: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    body: {
                        id: id
                    },
                    customHeaders: {
                        Accept: '*/*',
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                });

                fetchData({
                    url: "/Vote/MarkAsSolution",
                    options: fetchOptions,
                    timeOut: 60000
                })
                    .then(solutionMarkedUI)
                    .catch((error: any) => {

                        toast.show(`Error: ${error}`);

                    });


            }

            solutionLink?.addEventListener('click', markSolutionRequest);

        });

    };

    votePost = (): void => {

        const voteLinkContainers: Array<Element> = Array.from(document.getElementsByClassName('votelink-container'));

        voteLinkContainers.forEach((voteLinkContainer: Element) => {

            const containerParent: HTMLElement = voteLinkContainer.parentElement;
            const voteLink = <HTMLElement>voteLinkContainer.getElementsByClassName('votelink')[0];

            const handleVoteClick = (e: Event) => {

                e.preventDefault();
                e.stopImmediatePropagation();

                const voteType: string = voteLink.getAttribute('data-votetype');
                const isUpVote: boolean = voteType === 'up';
                const countIndicator = <HTMLElement>voteLinkContainer.getElementsByClassName('count')[0];
                const voteUrl: string = isUpVote ? "/Vote/VoteUpPost" : "/Vote/VoteDownPost";
                const hasVoted: boolean = voteLink.getAttribute('data-hasvoted') === "true";
                const toVoteText: string = voteLink.getAttribute('data-votetext');
                const votedText: string = voteLink.getAttribute('data-votedtext');
                const postId: string = voteLink.getAttribute('data-id');
                const oppositeType: string = isUpVote ? 'down' : 'up';
                const oppositeVoteLink: HTMLElement = containerParent.querySelector(`.votelink[data-votetype="${oppositeType}"]`);

                const updateVoteUI = () => {

                    let count: number = parseInt(countIndicator.innerText);

                    if (hasVoted) {

                        $(oppositeVoteLink).show();
                        countIndicator.innerText = (count - 1).toString();
                        voteLink.innerText = toVoteText;
                        voteLink.setAttribute('data-hasvoted', 'false');

                    } else {

                        $(oppositeVoteLink).hide();
                        countIndicator.innerText = (count + 1).toString();
                        voteLink.innerText = votedText;
                        voteLink.setAttribute('data-hasvoted', 'true');

                    }

                }

                const { setFetchOptions, fetchData } = this.fetchHelpers;
                const fetchOptions: FetchOptions = setFetchOptions({
                    method: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    body: {
                        Id: postId
                    },
                    customHeaders: {
                        Accept: '*/*',
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                });

                fetchData({
                    url: voteUrl,
                    options: fetchOptions,
                    timeOut: 60000
                })
                    .then(updateVoteUI)
                    .catch((error: any) => {

                        this.toast.show(`Error: ${error}`);

                    });
            }

            voteLink?.addEventListener('click', handleVoteClick);

        })

    }


    favouritePost = (): void => {

        const favouriteContainers: Array<Element> = Array.from(document.getElementsByClassName('favourite-container'));

        favouriteContainers.forEach((favouriteContainer: Element) => {

            const favouriteLink = <HTMLElement>favouriteContainer.getElementsByClassName('favourite')[0];

            const handleFavouriteClick = (e: Event) => {

                e.preventDefault();
                e.stopImmediatePropagation();

                const id: string = favouriteLink.getAttribute('data-id');
                const action: string = favouriteLink.getAttribute('data-addremove');
                const icon: Element = favouriteContainer.getElementsByClassName('c-ui-icon')[0];
                const countElement = <HTMLElement>favouriteContainer.getElementsByClassName('count')[0];
                const currentCount: number = parseInt(countElement.innerText);
                const isAdding: boolean = action === 'add';

                const updateFavouriteUI = (data: any) => {

                    favouriteLink.innerText = data.Message;

                    if (isAdding) {

                        countElement.innerText = (currentCount + 1).toString();
                        favouriteLink.setAttribute('data-addremove', 'remove');
                        icon.classList.add('c-ui-icon--yellow');
                        return;

                    }

                    countElement.innerText = (currentCount - 1).toString();
                    favouriteLink.setAttribute('data-addremove', 'add');
                    icon.classList.remove('c-ui-icon--yellow');

                }

                const { setFetchOptions, fetchData } = this.fetchHelpers;
                const fetchOptions: FetchOptions = setFetchOptions({
                    method: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    body: {
                        Id: id
                    },
                    customHeaders: {
                        Accept: '*/*',
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                });

                fetchData({
                    url: "/Favourite/FavouritePost",
                    options: fetchOptions,
                    timeOut: 60000
                })
                    .then(updateFavouriteUI)
                    .catch((error: any) => {

                        this.toast.show(`Error: ${error}`);

                    });

            }

            favouriteLink?.addEventListener('click', handleFavouriteClick);

        });

    }

    bindModerateForumEvents = (target: 'post' | 'topic'): void => {

        const toast = this.toast;
        const { setFetchOptions, fetchData } = this.fetchHelpers;
        const targetActionButtons: Array<Element> = Array.from(document.getElementsByClassName(`${target}action`));

        const handleClick = function (e: Event) {

            e.preventDefault();

            const id: string = this.getAttribute(`data-${target}id`);
            const action: string = this.getAttribute(`data-${target}action`);
            const snippetHolder: Element = document.getElementById(`${target}-${id}`);
            const isDeleteAction: boolean = action === 'delete';
            const isDeleteConfirmed = confirm('Are you sure?');

            if (isDeleteAction && !isDeleteConfirmed) {
                return;
            }

            const approve: boolean = isDeleteAction ? false : true;
            const actionCapitalized: string = target.charAt(0).toUpperCase() + target.slice(1);
            const moderateTargetAction = {
                IsApproved: approve,
                [actionCapitalized + 'Id']: id
            }

            const fetchOptions: FetchOptions = setFetchOptions({
                method: 'POST',
                body: moderateTargetAction
            });

            fetchData({
                url: `/Moderate/Moderate${actionCapitalized}`,
                options: fetchOptions,
                timeOut: 60000
            })
                .then((data: string) => {

                    if (data === "allgood") {
                        $(snippetHolder).fadeOut('fast');
                    } else {
                        toast.show(data);
                    }

                })
                .catch((error: any) => {

                    toast.show(`Error: ${error}`);

                });


        }

        targetActionButtons.forEach(targetActionButton => {


            targetActionButton.addEventListener('click', handleClick)

        });

    }

    public changeCreatePostUI = (enable: boolean = true): void => {

        const createPostBtn: Element = document.getElementById('createpostbutton');

        if (enable) {

            createPostBtn?.classList.remove('u-disabled');

            return;

        }

        createPostBtn?.classList.add('u-disabled');

    }

    public postRequestInitiate = (): void => {

        this.changeCreatePostUI(false);

    }

    public postRequestSuccess = (): void => {

        //the current implementation of MicrosoftMvcAjax.js requires this html logic 
        const postTarget: Element = document.getElementById('newpostmarker');
        postTarget.removeAttribute('id');
        postTarget.insertAdjacentHTML('afterend', '<div id="newpostmarker"></div>');

        window.tinyMCE?.activeEditor.setContent("");

        const bbEditorTextArea = <HTMLTextAreaElement>document.querySelector('.bbeditorholder textarea');
        const wwdInput = <HTMLInputElement>document.getElementsByClassName('wmd-input')[0];
        const wwdPreviewContainer = <HTMLElement>document.getElementsByClassName('wmd-preview')[0];
        const replyToContainer = <HTMLElement>document.getElementsByClassName('reshowreplyto')[0];

        if (bbEditorTextArea) {
            bbEditorTextArea.value = '';
        }

        if (wwdInput) {
            wwdInput.value = '';
        }

        if (wwdPreviewContainer) {
            wwdPreviewContainer.innerHTML = ''
        }

        if (replyToContainer) {
            replyToContainer.innerHTML = ''
        }

        this.bindFeaturesToPost();

        this.postRequestFinally();

    }

    public postRequestError = (errorMessage: string): void => {

        this.toast.show(errorMessage);

        this.postRequestFinally();

    }

    public postRequestFinally = (): void => {

        this.changeCreatePostUI();

    }

}
