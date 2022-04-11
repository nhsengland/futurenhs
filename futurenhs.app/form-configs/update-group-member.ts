import { formTypes } from '@constants/forms'
import { FormConfig } from '@appTypes/form'

export const updateGroupMemberForm: FormConfig = {
    id: formTypes.UPDATE_GROUP,
    steps: [
        {
            fields: [
                {
                    name: 'member-role',
                    inputType: 'multiChoice',
                    text: {
                        label: 'Member role',
                    },
                    options: [
                        {
                            value: 'Admin',
                            label: 'Admin',
                        },
                        {
                            value: 'Standard Members',
                            label: 'Member',
                        },
                    ],
                    component: 'multiChoice',
                    validators: [
                        {
                            type: 'required',
                            message: 'Select a member role',
                        },
                    ],
                },
            ],
        },
    ],
}
