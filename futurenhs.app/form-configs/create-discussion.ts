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
                        },
                        {
                            type: 'maxLength',
                            maxLength: 500,
                            message: 'Enter 500 or fewer characters'
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
                    shouldRenderRemainingCharacterCount: true,
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter the discussion comment'
                        },
                        {
                            type: 'maxLength',
                            maxLength: 4000,
                            message: 'Enter 4000 or fewer characters'
                        }
                    ]
                }
            ]
        }
    ]
};