import { formTypes } from '@constants/forms'
import { FormConfig } from '@appTypes/form'

export const rejectGroupMemberForm: FormConfig = {
    id: formTypes.REJECT_GROUP_MEMBER,
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
