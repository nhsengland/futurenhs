import { formTypes } from '@constants/forms'
import { FormConfig } from '@appTypes/form'

export const acceptGroupMemberForm: FormConfig = {
    id: formTypes.ACCEPT_GROUP_MEMBER,
    steps: [
        {
            fields: [
                {
                    name: 'MembershipUserId',
                    component: 'hidden'
                }
            ],
        },
    ],
}
