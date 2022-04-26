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
                        label: 'Subtitle',
                    },
                    component: 'input',
                    shouldRenderRemainingCharacterCount: true,
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter the subtitle',
                        },
                        {
                            type: 'maxLength',
                            maxLength: 255,
                            message: 'Enter 255 or fewer characters',
                        },
                    ],
                },
                {
                    name: 'mainText',
                    text: {
                        label: 'Main text',
                    },
                    component: 'textArea',
                    shouldRenderAsRte: true,
                    shouldRenderRemainingCharacterCount: true,
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter the main text',
                        },
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
