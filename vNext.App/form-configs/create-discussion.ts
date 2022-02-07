import { formTypes } from '@constants/forms';
import { Form } from '@appTypes/form';

export const createDiscussionForm: Form = {
    id: formTypes.CREATE_DISCUSSION,
    steps: [
        {
            fields: [
                {
                    name: 'discussion-title',
                    inputType: 'text',
                    text: {
                        label: 'Title'
                    },
                    component: 'input',
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter the discussion title'
                        }
                    ]
                },
                {
                    name: 'discussion-comment',
                    text: {
                        label: 'Comment'
                    },
                    component: 'textArea',
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter the discussion comment'
                        }
                    ]
                }
            ]
        }
    ]
};