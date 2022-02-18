import { formTypes } from '@constants/forms';
import { FormConfig } from '@appTypes/form';

export const createDiscussionForm: FormConfig = {
    id: formTypes.CREATE_DISCUSSION,
    steps: [
        {
            fields: [
                {
                    name: 'title',
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
                    name: 'content',
                    text: {
                        label: 'Comment'
                    },
                    component: 'textArea',
                    shouldRenderAsRte: true,
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