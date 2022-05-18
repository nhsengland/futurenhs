import { Props } from './interfaces'
import { useFormConfig } from '@hooks/useForm'
import { formTypes } from '@constants/forms'
import { FormConfig } from '@appTypes/form'
import { useCsrf } from '@hooks/useCsrf'
import { Form } from '@components/Form'


export const PendingMemberActions: (props: Props) => JSX.Element = ({
    acceptAction,
    rejectAction,
    text,
    memberId
}) => {

    const { acceptMember, rejectMember } = text

    const acceptForm: FormConfig = useFormConfig(formTypes.ACCEPT_GROUP_MEMBER, { MembershipUserId: memberId })
    const rejectForm: FormConfig = useFormConfig(formTypes.REJECT_GROUP_MEMBER, { MembershipUserId: memberId })
    const csrfToken: string = useCsrf()


    return (
        <span className="u-flex u-justify-between u-w-full">
            <Form
                csrfToken={csrfToken}
                formConfig={acceptForm}
                text={{
                    submitButton: acceptMember,
                }}
                submitAction={acceptAction}
                submitButtonClassName="u-mb-0"
            />
            <Form
                csrfToken={csrfToken}
                formConfig={rejectForm}
                text={{
                    submitButton: rejectMember,
                }}
                submitAction={rejectAction}
                submitButtonClassName="c-button-outline u-mb-0"
            />
        </span>
    )

}
