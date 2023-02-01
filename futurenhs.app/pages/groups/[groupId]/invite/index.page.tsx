import { GetServerSideProps } from 'next'
import { useContext, useState } from 'react'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { actions as actionConstants } from '@constants/actions'
import {
    features,
    groupTabIds,
    layoutIds,
    routeParams,
} from '@constants/routes'
import { withUser } from '@helpers/hofs/withUser'
import { withRoutes } from '@helpers/hofs/withRoutes'
import { GetServerSidePropsContext } from '@appTypes/next'
import { withTextContent } from '@helpers/hofs/withTextContent'
import { withGroup } from '@helpers/hofs/withGroup'
import { formTypes } from '@constants/forms'
import {
    selectCsrfToken,
    selectFormData,
    selectParam,
    selectRequestMethod,
    selectUser,
    selectPageProps,
} from '@helpers/selectors/context'
import { User } from '@appTypes/user'
import { getGenericFormError, ServerSideFormData } from '@helpers/util/form'
import { requestMethods } from '@constants/fetch'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { notifications } from '@constants/notifications'
import { FormConfig, FormErrors } from '@appTypes/form'
import { FormWithErrorSummary } from '@components/forms/FormWithErrorSummary'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { NotificationsContext } from '@helpers/contexts/index'
import { useFormConfig } from '@helpers/hooks/useForm'
import { useNotification } from '@helpers/hooks/useNotification'
import { GroupPage } from '@appTypes/page'
import { postGroupUserInvite } from '@services/postGroupUserInvite'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { getFeatureEnabled } from '@services/getFeatureEnabled'
import { Input } from '@components/forms/Input'
import { ValidationMessage } from '@helpers/validation'
import { ErrorSummary } from '@components/generic/ErrorSummary'
import classNames from 'classnames'
import { RemainingCharacterCount } from '@components/forms/RemainingCharacterCount'

export interface Props extends GroupPage {}

/**
 * Group member invite template
 */
export const GroupMemberInvitePage: (props: Props) => JSX.Element = ({
    csrfToken,
    forms,
    user,
    contentText,
    groupId,
}) => {
    const [validationErrors, setValidationErrors] = useState<
        Array<ValidationMessage>
    >([])
    const notificationsContext: any = useContext(NotificationsContext)

    /**
     * Client-side submission handler - TODO: Pending API
     */
    const handleSubmit = async (formData: FormData) => {
        const headers = getStandardServiceHeaders({
            csrfToken,
            accessToken: user.accessToken,
        })
        try {
            await postGroupUserInvite({
                user,
                headers,
                body: formData as any,
                groupId,
            })

            const emailAddress: FormDataEntryValue = formData.get('Email')
            useNotification({
                notificationsContext,
                text: {
                    heading: notifications.SUCCESS,
                    body: `Invite sent to ${emailAddress}`,
                },
            })
        } catch (error) {}
    }
    const errorClasses: any = {
        wrapper: 'c-error-summary',
        list: classNames(
            'c-error-summary_list',
            'u-m-0',
            'u-p-0',
            'u-list-none'
        ),
        listItem: classNames('c-error-summary_list-item'),
        link: classNames('c-error-summary_link'),
        item: classNames('c-error-summary_item'),
    }

    const inputClasses: any = {
        wrapper: classNames('nhsuk-form-group', 'nhsuk-form-group--error'),
        label: classNames('nhsuk-label'),
        hint: classNames('nhsuk-hint'),
        error: classNames('nhsuk-error-message'),
        input: classNames(
            'nhsuk-input nhsuk-u-width-full',
            'nhsuk-input--error'
        ),
    }

    const charCountClasses = {
        wrapper: classNames('nhsuk-hint', 'u-mt-1', 'u-mb-0'),
    }
    /**
     * Render
     */
    return (
        <LayoutColumn className="c-page-body">
            <LayoutColumnContainer>
                <LayoutColumn tablet={8}>
                    {/* <div
                        aria-live="assertive"
                        aria-atomic="true"
                        aria-relevant="additions"
                        tabIndex={-1}
                    >
                        <div className={errorClasses.wrapper}>
                            There is a problem
                            <ul className={errorClasses.list}>
                                <li className={errorClasses.listItem}>
                                    <span className={errorClasses.item}>
                                        Enter a valid email address
                                    </span>
                                </li>
                            </ul>
                        </div>
                    </div> */}
                    <div className={inputClasses.wrapper}>
                        <label className={inputClasses.label}>
                            Email address
                        </label>
                        {/* <span className={inputClasses.hint}>Test hint</span> */}
                        <span className={inputClasses.error}>
                            Enter a valid email address
                        </span>
                        <input type="text" className={inputClasses.input} />
                        {/* <span
                            aria-live="polite"
                            className={charCountClasses.wrapper}
                        >
                            {true
                                ? 'characters too many'
                                : 'characters remaining'}
                        </span> */}
                    </div>
                </LayoutColumn>
            </LayoutColumnContainer>
        </LayoutColumn>
    )
}

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (
    context: GetServerSidePropsContext
) =>
    await pipeSSRProps(
        context,
        {
            routeId: 'f872b71a-0449-4821-a8da-b75bbd451b2d',
        },
        [withUser, withRoutes, withGroup, withTextContent],
        async (context: GetServerSidePropsContext) => {
            /**
             * Get data from request context
             */
            const props: Partial<Props> = selectPageProps(context)
            const formData: ServerSideFormData = selectFormData(context)
            const requestMethod: requestMethods = selectRequestMethod(context)

            props.layoutId = layoutIds.GROUP
            props.tabId = groupTabIds.MEMBERS
            props.pageTitle = `${props.entityText.title} - ${props.contentText.subTitle}`

            props.forms = {
                [formTypes.INVITE_USER]: {},
            }
            /**
             * Return page not found if user doesn't have permissions to invite a user - TODO: Pending API
             */
            const hasPermisson = props.actions?.includes(
                actionConstants.GROUPS_MEMBERS_INVITE
            )

            let groupInviteEnabled = false

            try {
                const { data: enabled } = await getFeatureEnabled({
                    slug: features.GROUP_INVITE,
                })
                groupInviteEnabled = enabled
            } catch (e) {}

            if (!hasPermisson || !groupInviteEnabled) {
                return {
                    notFound: true,
                }
            }

            /**
             * Return data to page template
             */
            return handleSSRSuccessProps({ props, context })
        }
    )

/**
 * Export page template
 */
export default GroupMemberInvitePage
