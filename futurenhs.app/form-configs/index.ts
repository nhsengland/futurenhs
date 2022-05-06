import { createDiscussionForm } from './create-discussion';
import { createDiscussionCommentForm } from './create-discussion-comment';
import { createDiscussionCommentReplyForm } from './create-discussion-comment-reply';
import { groupFolderForm } from './group-folder';
import { createFileForm } from './create-file';
import { createGroupForm } from './create-group';
import { updateGroupForm } from './update-group';
import { deleteGroupMemberForm } from './delete-group-member';
import { updateGroupMemberForm } from './update-group-member';
import { inviteUserForm } from './invite-user';
import { updateSiteMemberForm } from './update-site-user';
import { updateSiteUserRoleForm } from './update-site-user-role';
import { contentBlockTextForm } from './content-block-text';
import { contentBlockQuickLinksWrapper } from './content-block-quick-links-wrapper';
import { contentBlockQuickLink } from './content-block-quick-link';
import { formTypes } from '@constants/forms';

export default {
    [formTypes.CREATE_DISCUSSION]: createDiscussionForm,
    [formTypes.CREATE_DISCUSSION_COMMENT]: createDiscussionCommentForm,
    [formTypes.CREATE_DISCUSSION_COMMENT_REPLY]: createDiscussionCommentReplyForm,
    [formTypes.CREATE_FILE]: createFileForm,
    [formTypes.GROUP_FOLDER]: groupFolderForm,
    [formTypes.CREATE_GROUP]: createGroupForm,
    [formTypes.UPDATE_GROUP]: updateGroupForm,
    [formTypes.UPDATE_GROUP_MEMBER]: updateGroupMemberForm,
    [formTypes.INVITE_USER]: inviteUserForm,
    [formTypes.UPDATE_SITE_USER]: updateSiteMemberForm,
    [formTypes.UPDATE_SITE_USER_ROLE]: updateSiteUserRoleForm,
    [formTypes.CONTENT_BLOCK_TEXT]: contentBlockTextForm,
    [formTypes.CONTENT_BLOCK_QUICK_LINKS_WRAPPER]: contentBlockQuickLinksWrapper,
    [formTypes.CONTENT_BLOCK_QUICK_LINK]: contentBlockQuickLink,
    [formTypes.DELETE_GROUP_MEMBER]: deleteGroupMemberForm
}
