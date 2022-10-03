import { formTypes } from '@constants/forms'
import { FormConfig } from '@appTypes/form'

export const updateGroupMemberForm: FormConfig = {
    id: formTypes.UPDATE_GROUP_MEMBER,
    steps: [
        {
            fields: [
                {
                    name: 'groupUserRoleId',
                    inputType: 'multiChoice',
                    text: {
                        label: 'Member role',
                    },
                    options: [
                        /**
                         * List of options dynamically generated from response from API
                         */
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
