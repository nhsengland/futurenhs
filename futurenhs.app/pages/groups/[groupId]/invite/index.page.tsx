import { GetServerSideProps } from 'next'
import { useContext, useState } from 'react'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { actions as actionConstants } from '@constants/actions'
import { groupTabIds, layoutIds, routeParams } from '@constants/routes'
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
import { flags } from '@constants/feature-flags'

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
    const formConfig: FormConfig = useFormConfig(
        formTypes.INVITE_USER,
        forms[formTypes.INVITE_USER]
    )
    const [errors, setErrors] = useState(formConfig?.errors)
    const notificationsContext: any = useContext(NotificationsContext)

    const { secondaryHeading } = contentText

    /**
     * Client-side submission handler - TODO: Pending API
     */
    const handleSubmit = async (formData: FormData): Promise<FormErrors> => {
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

            return Promise.resolve({})
        } catch (error) {
            const errors: FormErrors =
                getServiceErrorDataValidationErrors(error) ||
                getGenericFormError(error)

            setErrors(errors)

            return Promise.resolve(errors)
        }
    }

    /**
     * Render
     */
    return (
        <LayoutColumn className="c-page-body">
            <LayoutColumnContainer>
                <LayoutColumn tablet={8}>
                    <FormWithErrorSummary
                        csrfToken={csrfToken}
                        formConfig={formConfig}
                        errors={errors}
                        text={{
                            form: {
                                submitButton: 'Send invite',
                            },
                        }}
                        submitAction={handleSubmit}
                        shouldClearOnSubmitSuccess={true}
                    >
                        <h2 className="nhsuk-heading-l">{secondaryHeading}</h2>
                    </FormWithErrorSummary>
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
            const props: Partial<any> = selectPageProps(context)
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

            const featureEnabled = props.featureFlags.some(
                (f) => f.id === flags.groupInvite && f.enabled
            )

            if (!hasPermisson || !featureEnabled) {
                return {
                    notFound: true,
                }
            }

            /**
             * Handle server-side form post
             */
            if (formData && requestMethod === requestMethods.POST) {
                props.forms[formTypes.INVITE_USER].initialValues = formData

                try {
                    const emailAddress: string = formData.get('Email')
                    props.notifications = [
                        {
                            heading: notifications.SUCCESS,
                            main: `Invite sent to ${emailAddress}`,
                        },
                    ]

                    return {
                        props: props,
                    }
                } catch (error) {
                    const validationErrors: FormErrors =
                        getServiceErrorDataValidationErrors(error)

                    if (validationErrors) {
                        props.forms[formTypes.INVITE_USER].errors =
                            validationErrors
                    } else {
                        return handleSSRErrorProps({ props, error })
                    }
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
