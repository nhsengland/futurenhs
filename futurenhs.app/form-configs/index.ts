import { createDiscussionForm } from './create-discussion';
import { createDiscussionCommentForm } from './create-discussion-comment';
import { createDiscussionCommentReplyForm } from './create-discussion-comment-reply';
import { groupFolderForm } from './group-folder';
import { createFileForm } from './create-file';
import { updateGroupForm } from './update-group';
import { updateGroupMemberForm } from './update-group-member';
import { formTypes } from '@constants/forms';

export default {
    [formTypes.CREATE_DISCUSSION]: createDiscussionForm,
    [formTypes.CREATE_DISCUSSION_COMMENT]: createDiscussionCommentForm,
    [formTypes.CREATE_DISCUSSION_COMMENT_REPLY]: createDiscussionCommentReplyForm,
    [formTypes.CREATE_FILE]: createFileForm,
    [formTypes.GROUP_FOLDER]: groupFolderForm,
    [formTypes.UPDATE_GROUP]: updateGroupForm,
    [formTypes.UPDATE_GROUP_MEMBER]: updateGroupMemberForm
};