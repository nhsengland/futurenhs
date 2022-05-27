import { formTypes } from '@constants/forms'
import { FormConfig } from '@appTypes/form'

export const contentBlockQuickLinksWrapper: FormConfig = {
    id: formTypes.CONTENT_BLOCK_QUICK_LINKS_WRAPPER,
    steps: [
        {
            fields: [
                {
                    name: 'title',
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
            ],
        },
    ],
}
