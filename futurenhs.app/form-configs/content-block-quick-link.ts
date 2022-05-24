import { formTypes } from '@constants/forms'
import { FormConfig } from '@appTypes/form'

export const contentBlockQuickLink: FormConfig = {
    id: formTypes.CONTENT_BLOCK_QUICK_LINK,
    steps: [
        {
            fields: [
                {
                    name: 'linkText',
                    inputType: 'text',
                    text: {
                        label: 'Link title',
                    },
                    component: 'input',
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter the link title',
                        },
                        {
                            type: 'maxLength',
                            maxLength: 30,
                            message: 'Enter 30 or fewer characters',
                        }
                    ],
                    className: 'u-grow tablet:u-mr-3'
                },
                {
                    name: 'url',
                    inputType: 'text',
                    text: {
                        label: 'Link',
                    },
                    component: 'input',
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter the link',
                        }
                    ],
                    className: 'u-grow'
                }
            ],
        },
    ],
}
