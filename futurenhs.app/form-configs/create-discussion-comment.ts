import { formTypes } from '@constants/forms';
import { FormConfig } from '@appTypes/form';

export const createDiscussionCommentForm: FormConfig = {
    id: formTypes.CREATE_DISCUSSION_COMMENT,
    steps: [
        {
            fields: [
                {
                    name: 'content',
                    text: {
                        label: 'Your comment'
                    },
                    component: 'textArea',
                    shouldRenderAsRte: true,
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter your comment'
                        }
                    ]
                }
            ]
        }
    ]
};