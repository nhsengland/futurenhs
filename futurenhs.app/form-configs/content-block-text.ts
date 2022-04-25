import { formTypes } from '@constants/forms'
import { FormConfig } from '@appTypes/form'

export const contentBlockTextForm: FormConfig = {
    id: formTypes.CONTENT_BLOCK_TEXT,
    steps: [
        {
            fields: [
                {
                    name: 'heading',
                    inputType: 'text',
                    text: {
                        label: 'Enter a block heading',
                    },
                    component: 'input',
                    shouldRenderRemainingCharacterCount: true,
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter the block heading',
                        },
                        {
                            type: 'maxLength',
                            maxLength: 200,
                            message: 'Enter 200 or fewer characters',
                        },
                    ],
                },
                {
                    name: 'body',
                    text: {
                        label: 'Enter a block body (optional)',
                    },
                    component: 'textArea',
                    shouldRenderRemainingCharacterCount: true,
                    validators: [
                        {
                            type: 'maxLength',
                            maxLength: 4000,
                            message: 'Enter 4000 or fewer characters',
                        },
                    ],
                },
            ],
        },
    ],
}
