import { formTypes } from "@constants/forms";
import { FormConfig } from '@appTypes/form';

export const updateGroupMemberForm: FormConfig = {
    id: formTypes.UPDATE_GROUP,
    steps: [
        {
            fields: [
                {
                    name: 'member-role',
                    inputType: 'multiChoice',
                    text: {
                        label: 'Member role'
                    },
                    options: [
                        {
                            value: 'admin',
                            label: 'Admin'
                        },
                        {
                            value: 'member',
                            label: 'Member'
                        }
                    ],
                    component: 'multiChoice',
                    validators: [
                        {
                            type: 'required',
                            message: 'Select whether the group should include a forum page'
                        }
                    ]
                }
            ]
        }
    ]
};