import { formTypes } from '@constants/forms';
import { FormConfig } from '@appTypes/form';

export const createDiscussionCommentReplyForm: FormConfig = {
    id: formTypes.CREATE_DISCUSSION_COMMENT_REPLY,
    steps: [
        {
            fields: [
                {
                    name: 'content',
                    text: {
                        label: 'Your reply'
                    },
                    component: 'textArea',
                    shouldRenderAsRte: true,
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter your reply'
                        }
                    ]
                }
            ]
        }
    ]
};