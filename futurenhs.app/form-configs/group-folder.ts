import { formTypes } from '@constants/forms'
import { FormConfig } from '@appTypes/form'

export const groupFolderForm: FormConfig = {
    id: formTypes.GROUP_FOLDER,
    steps: [
        {
            fields: [
                {
                    name: 'Name',
                    inputType: 'text',
                    text: {
                        label: 'Enter a folder title',
                    },
                    component: 'input',
                    shouldRenderRemainingCharacterCount: true,
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter the folder title',
                        },
                        {
                            type: 'maxLength',
                            maxLength: 200,
                            message: 'Enter 200 or fewer characters',
                        },
                    ],
                },
                {
                    name: 'Description',
                    text: {
                        label: 'Enter a folder description (optional)',
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
