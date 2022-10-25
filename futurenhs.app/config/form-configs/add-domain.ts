import { formTypes } from '@constants/forms'
import { FormConfig } from '@appTypes/form'

export const addDomainForm: FormConfig = {
    id: formTypes.ADD_DOMAIN,
    steps: [
        {
            fields: [
                {
                    name: 'Domain',
                    inputType: 'text',
                    text: {
                        label: 'Email domain',
                    },
                    component: 'input',
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter a domain',
                        },
                    ],
                },
            ],
        },
    ],
}
