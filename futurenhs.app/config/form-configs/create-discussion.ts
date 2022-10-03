import { formTypes } from '@constants/forms'
import { FormConfig } from '@appTypes/form'

export const createDiscussionForm: FormConfig = {
    id: formTypes.CREATE_DISCUSSION,
    steps: [
        {
            fields: [
                {
                    name: 'Title',
                    inputType: 'text',
                    text: {
                        label: 'Title',
                    },
                    component: 'input',
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter the discussion title',
                        },
                        {
                            type: 'maxLength',
                            maxLength: 100,
                            message: 'Enter 100 or fewer characters',
                        },
                    ],
                },
                {
                    name: 'Content',
                    text: {
                        label: 'Comment',
                    },
                    component: 'textArea',
                    shouldRenderAsRte: true,
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter the discussion comment',
                        },
                        {
                            type: 'maxLength',
                            maxLength: 100000,
                            message: 'Enter 100,000 or fewer characters',
                        },
                    ],
                },
            ],
        },
    ],
}
