import { TopicPost } from '@modules/ui/components/topicPost';
import { Toast } from '@modules/ui/components/toast';
import * as fetchHelpers from '@utilities/fetch';


const toastElement: HTMLElement = document.getElementById('js-toast');

const topicPost = new TopicPost({
    wrapperSelector: (document as any)
}, {
    fetchHelpers: fetchHelpers,
    components: {
        toast: new Toast({
            wrapperSelector: toastElement
        })
    }
});


export const topics = {
    changeCreatePostUI: topicPost.changeCreatePostUI,
    postRequestInitiate: topicPost.postRequestInitiate,
    postRequestSuccess: topicPost.postRequestSuccess,
    postRequestError: topicPost.postRequestError,
    postRequestFinally: topicPost.postRequestFinally,
    bindFeaturesToPost: topicPost.bindFeaturesToPost
}