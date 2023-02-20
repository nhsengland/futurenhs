import { formTypes } from '@constants/forms'
import { FormConfig } from '@appTypes/form'

export const inviteUserForm: FormConfig = {
    id: formTypes.INVITE_USER,
    steps: [
        {
            fields: [
                {
                    name: 'Email',
                    inputType: 'multi',
                    text: {
                        label: 'Email address',
                    },
                    component: 'input',
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter an email address',
                        },
                        {
                            type: 'email',
                            message: 'Enter a valid email address',
                        },
                    ],
                },
            ],
        },
    ],
}
