import { UIComponentBase } from '@modules/ui/componentBase';
import { Toast } from '@modules/ui/components/toast';
import * as fetchHelpers from '@utilities/fetch';
import { FetchOptions } from '@appTypes/fetch';

interface Config {
    wrapperSelector: HTMLElement;
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


    wrapperSelector: HTMLElement = undefined;
    toast: Toast = undefined;
    fetchHelpers: typeof fetchHelpers = undefined;

    constructor(config: Config, dependencies: Dependencies) {

        super(config, dependencies);

        this.wrapperSelector = config.wrapperSelector;
        this.toast = dependencies.components.toast;
        this.fetchHelpers = dependencies.fetchHelpers;

        this.init();
    }


    init = () => {

        this.votePost();
        this.voteInPoll();
        this.getAllLikes();
        this.favouritePost();
        
        /* 
            depricated logic
            this is replaced by the new load more functionality 
        */
        // this.showMorePosts();
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

    public bindFeaturesToPost = () => {

        this.votePost();
        this.favouritePost();
        this.showPostOptions();
        this.markPostAsSolution();
        this.displayFileUploader();
        this.displayLoadingAttachments();

    }


    voteInPoll = () => {

        const wrapperSelector = this.wrapperSelector;
        const pollVoteButton = <HTMLAnchorElement>wrapperSelector.getElementsByClassName('pollvotebutton')?.[0];
        const handleVoteInPoll = (e: Event): void => {

            e.preventDefault();

            const pollId: string = (<HTMLInputElement>document.getElementById('Poll_Id'))?.value;
            const answerId: string = (<HTMLInputElement>wrapperSelector.getElementsByClassName('selectedpollanswer')?.[0])?.value;

            const updatePollViewModel = {
                PollId: pollId,
                AnswerId: answerId
            };

            $.ajax({
                url: "/Poll/UpdatePoll",
                type: "POST",
                dataType: "html",
                data: JSON.stringify(updatePollViewModel),
                cache: false,
                contentType: "application/json; charset=utf-8",
                success: (data) => {
                    const pollContainer = wrapperSelector.getElementsByClassName('pollcontainer')[0];
                    pollContainer.innerHTML = data;
                },
                error: (xhr, ajaxOptions, thrownError) => {
                    this.toast.show("Error: " + xhr.status + " " + thrownError);
                }
            });


        }

        pollVoteButton?.addEventListener('click', handleVoteInPoll);

    }

    getAllLikes = () => {

        const wrapperSelector = this.wrapperSelector;
        const likedByContainers: Array<Element> = Array.from(wrapperSelector.getElementsByClassName("postlikedby"));

        likedByContainers.forEach((likedByContainer: Element) => {

            const othersLikedItem = <HTMLLIElement>likedByContainer.getElementsByClassName("othersliked")[0];

            if (!othersLikedItem) {
                return;
            }

            const othersLikedItemLink = <HTMLAnchorElement>othersLikedItem.getElementsByTagName('a')[0];
            const id = othersLikedItem.getAttribute('data-postid');

            const handleOnClick = (e: Event) => {

                e.preventDefault();

                $.ajax({
                    url: "/Post/GetAllPostLikes",
                    type: "POST",
                    dataType: "html",
                    cache: false,
                    data: { id: id },
                    success: (data: string) => {

                        othersLikedItem.innerHTML = data;

                    },
                    error: (xhr, ajaxOptions, thrownError) => {
                        this.toast.show("Error: " + xhr.status + " " + thrownError);
                    }
                });



            }

            othersLikedItemLink.addEventListener('click', handleOnClick);


        })


    }

    displayFileUploader = (): void => {

        const wrapperSelector = this.wrapperSelector;
        const showAttachedContainers: Array<Element> = Array.from(wrapperSelector.getElementsByClassName('postshowattach'));

        const handleOnClick = function (e: Event): void {

            e.preventDefault();

            const postHolder: Element = (this as HTMLAnchorElement).closest('div.post');
            const uploadHolder: Element = postHolder.getElementsByClassName('postuploadholder')[0];

            $(uploadHolder).toggle();

        }

        showAttachedContainers.forEach((showAttachedContainer: HTMLAnchorElement) => {

            showAttachedContainer.addEventListener('click', handleOnClick);

        });


    }

    showPostOptions = (): void => {

        const wrapperSelector = this.wrapperSelector;
        const postOptions: Array<Element> = Array.from(wrapperSelector.getElementsByClassName('postoptions'));

        const handleOnClick = function (e: Event): void {

            e.preventDefault();

            const adminPostHolder: Element = (this as HTMLButtonElement).closest('div.postadmin');
            const postAdminList: Element = adminPostHolder.getElementsByClassName('postadminlist')[0];

            $(postAdminList).toggle();

        }

        postOptions.forEach((postOption) => {

            postOption.addEventListener('click', handleOnClick);

        });

    }

    selectPollAnswer = (): void => {

        const wrapperSelector = this.wrapperSelector;
        const pollContainer = wrapperSelector.getElementsByClassName('pollcontainer')[0];
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

        const fileTypes: Array<string> = ['gif', 'jpg', 'png', 'bmp', 'jpeg'];
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

    showMorePosts = (): void => {

        const toast = this.toast;
        const showMoreAnchors: Array<Element> = Array.from(document.getElementsByClassName('showmoreposts'));
        const displayFileUploader = this.displayFileUploader;

        const handleOnClick = function (e: Event) {

            e.preventDefault();

            const showMore = (this as HTMLAnchorElement);
            const topicInput = <HTMLInputElement>document.getElementById('topicId');
            const topicId: string = topicInput.value;

            const pageIndexInput = <HTMLInputElement>document.getElementById('pageIndex');
            const pageIndex = pageIndexInput.value;

            const totalPagesInput = <HTMLInputElement>document.getElementById('totalPages');
            const totalPages = parseInt(totalPagesInput.value);

            const activeText = showMore.getElementsByClassName('smpactive')[0];
            const loadingText = showMore.getElementsByClassName('smploading')[0];

            $(activeText).hide();
            $(loadingText).show();

            const getMorePosts = {
                TopicId: topicId,
                PageIndex: pageIndex,
                Order: totalPages
            };

            $.ajax({
                url: '/Topic/AjaxMorePosts',
                type: 'POST',
                dataType: 'html',
                data: JSON.stringify(getMorePosts),
                cache: false,
                contentType: 'application/json; charset=utf-8',
                success: function (data) {

                    showMore.insertAdjacentHTML('beforebegin', data);

                    // Update the page index value
                    var newPageIdex = parseInt(pageIndex) + 1;
                    pageIndexInput.value = newPageIdex.toString();

                    // If the new pageindex is greater than the total pages, then hide the show more button
                    if (newPageIdex > totalPages) {

                        $(showMore).hide();

                    }

                    // Lastly reattch the click events
                    //AddPostClickEvents();
                    displayFileUploader();

                    $(activeText).show();
                    $(loadingText).hide();

                },
                error: function (xhr, ajaxOptions, thrownError) {

                    toast.show("Error: " + xhr.status + " " + thrownError);
                    $(activeText).show();
                    $(loadingText).hide();


                }

            });

        }

        showMoreAnchors.forEach((showMore: HTMLAnchorElement) => {

            showMore.addEventListener('click', handleOnClick);

        });

    }

    markPostAsSolution = (): void => {

        const toast = this.toast;
        const wrapperSelector = this.wrapperSelector;
        const postSocialContainers: Array<Element> = Array.from(wrapperSelector.getElementsByClassName('postsocial'));
        const allSolutionLinks: Array<Element> = Array.from(wrapperSelector.getElementsByClassName('solutionlink'));
        const allSolutionIcons: Array<Element> = Array.from(wrapperSelector.getElementsByClassName('solution-icon'));

        postSocialContainers.forEach((socialContainer: HTMLAnchorElement) => {

            const solutionLink = <HTMLAnchorElement>socialContainer.getElementsByClassName('solutionlink')?.[0];

            if (!solutionLink) {
                return;
            }

            const solutionIcon = <SVGElement>socialContainer.getElementsByClassName('solution-icon')[0];

            const markSolutionRequest = function (e: Event) {

                e.preventDefault();

                const id: string = this.getAttribute('data-id');

                $.ajax({
                    url: "/Vote/MarkAsSolution",
                    type: "POST",
                    cache: false,
                    data: JSON.stringify({ id }),
                    contentType: "application/json; charset=utf-8",
                    success: solutionMarkedUI,
                    error: function (xhr, ajaxOptions, thrownError) {
                        toast.show("Error: " + xhr.status + " " + thrownError);
                    }
                });

            }

            const solutionMarkedUI = function () {

                solutionLink.removeEventListener('click', markSolutionRequest);

                allSolutionLinks.forEach((solutionLink: HTMLAnchorElement) => solutionLink.classList.add('u-visually-hidden'));
                allSolutionIcons.forEach((solutionIcon: SVGElement) => solutionIcon.classList.add('u-visually-hidden'));

                solutionIcon.classList.remove('u-visually-hidden');
                solutionIcon.classList.add('c-ui-icon--green');


            }

            solutionLink.addEventListener('click', markSolutionRequest);

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

                /* TO INSPECT - FETCH DECLINED INSTEAD OF QUERY POST  */

                // const { setFetchOptions, fetchData } = this.fetchHelpers;
                // const fetchOptions: FetchOptions = setFetchOptions({
                //     method: 'POST',
                //     contentType: 'application/json; charset=utf-8',
                //     body: {
                //         Id: postId
                //     },
                //     customHeaders: {
                //         Accept : '*/*',
                //     }
                // });

                // fetchData({
                //     url: voteUrl,
                //     options: fetchOptions,
                //     timeOut: 60000
                // }).catch((error: any) => {

                //     this.toast.show(`Error: ${error}`);

                // });

                $.ajax({
                    url: voteUrl,
                    type: "POST",
                    cache: false,
                    data: JSON.stringify({
                        Id: postId
                    }),
                    contentType: "application/json; charset=utf-8",
                    success: updateVoteUI,
                    error: function (xhr, ajaxOptions, thrownError) {
                        this.toast.show("Error: " + xhr.status + " " + thrownError);
                    }
                });
            }

            voteLink.addEventListener('click', handleVoteClick);

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
                const currentCount : number = parseInt(countElement.innerText);
                const isAdding: boolean = action === 'add';

                const updateFavouriteUI = (data: any) => {

                    favouriteLink.innerText = data.Message;
                    
                    if(isAdding){

                        countElement.innerText = (currentCount + 1).toString();
                        favouriteLink.setAttribute('data-addremove', 'remove');
                        icon.classList.add('c-ui-icon--yellow');
                        return;

                    }

                    countElement.innerText = (currentCount - 1).toString();
                    favouriteLink.setAttribute('data-addremove', 'add');
                    icon.classList.remove('c-ui-icon--yellow');

                }

                /* TO INSPECT - FETCH DECLINED INSTEAD OF QUERY POST  */
                //see :475 (fn votePost)

                $.ajax({
                    url: "/Favourite/FavouritePost",
                    type: "POST",
                    cache: false,
                    data: JSON.stringify({
                        Id: id
                    }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: updateFavouriteUI,
                    error: (xhr, ajaxOptions, thrownError) => {
                        this.toast.show("Error: " + xhr.status + " " + thrownError);
                    }
                });



            }
            
            favouriteLink.addEventListener('click', handleFavouriteClick);

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
