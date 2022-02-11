import { createDiscussionForm } from './create-discussion';
import { createDiscussionCommentForm } from './create-discussion-comment';
import { createDiscussionCommentReplyForm } from './create-discussion-comment-reply';
import { createFolderForm } from './create-folder';
import { formTypes } from '@constants/forms';

export default {
    [formTypes.CREATE_DISCUSSION]: createDiscussionForm,
    [formTypes.CREATE_DISCUSSION_COMMENT]: createDiscussionCommentForm,
    [formTypes.CREATE_DISCUSSION_COMMENT_REPLY]: createDiscussionCommentReplyForm,
    [formTypes.CREATE_FOLDER]: createFolderForm
};