import { formTypes } from '@constants/forms'
import { FormConfig } from '@appTypes/form'

export const updateSiteUserRoleForm: FormConfig = {
    id: formTypes.UPDATE_SITE_USER_ROLE,
    steps: [
        {
            fields: [
                {
                    name: 'newRoleId',
                    inputType: 'multiChoice',
                    text: {
                        label: "Choose user's platform role",
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
                            message: 'Select a user role',
                        },
                    ],
                },
                {
                    name: 'currentRoleId',
                    component: 'hidden',
                },
            ],
        },
    ],
}
