import { formTypes } from '@constants/forms'
import { FormConfig } from '@appTypes/form'

export const contentBlockTextForm: FormConfig = {
    id: formTypes.CONTENT_BLOCK_TEXT,
    steps: [
        {
            fields: [
                {
                    name: 'title',
                    inputType: 'text',
                    text: {
                        label: 'Subtitle',
                        hint: 'Add a title to the block e.g "Welcome"'
                    },
                    component: 'input',
                    shouldRenderRemainingCharacterCount: true,
                    validators: [
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
                        hint: 'This could be an introduction to your group, remember to include key words so that this group can be found in search'
                    },
                    component: 'textArea',
                    rteToolBarOptions: 'bold italic underline | bullist numlist | hr | link unlink',
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
