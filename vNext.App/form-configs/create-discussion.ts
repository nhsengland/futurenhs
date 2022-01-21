import { Form } from '@appTypes/form';

export const createDiscussionForm: Form = {
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
                    component: 'textArea'
                }
            ]
        }
    ]
};