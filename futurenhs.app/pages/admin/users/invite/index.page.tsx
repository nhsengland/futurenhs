import { GetServerSideProps } from 'next'
import { useContext, useState } from 'react'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { requestMethods } from '@constants/fetch'
import { actions as actionConstants } from '@constants/actions'
import { layoutIds } from '@constants/routes'
import { withUser } from '@helpers/hofs/withUser'
import { withRoutes } from '@helpers/hofs/withRoutes'
import {
    selectFormData,
    selectRequestMethod,
    selectPageProps,
} from '@helpers/selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { withTextContent } from '@helpers/hofs/withTextContent'
import { ServerSideFormData } from '@helpers/util/form'
import { getGenericFormError } from '@helpers/util/form'
import { formTypes } from '@constants/forms'
import { FormWithErrorSummary } from '@components/forms/FormWithErrorSummary'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { postSiteUserInvite } from '@services/postSiteUserInvite'
import { FormConfig, FormErrors } from '@appTypes/form'
import { useFormConfig } from '@helpers/hooks/useForm'
import { GroupPage } from '@appTypes/page'
import { useNotification } from '@helpers/hooks/useNotification'
import { NotificationsContext } from '@helpers/contexts/index'
import { notifications } from '@constants/notifications'

export interface Props extends GroupPage {
    folderId: string
}

/**
 * Admin invite user template
 */
export const AdminUsersInvitePage: (props: Props) => JSX.Element = ({
    csrfToken,
    forms,
    routes,
    user,
    contentText,
    services = {
        postSiteUserInvite: postSiteUserInvite,
    },
}) => {
    const formConfig: FormConfig = useFormConfig(
        formTypes.INVITE_USER,
        forms[formTypes.INVITE_USER]
    )
    const [errors, setErrors] = useState(formConfig?.errors)
    const notificationsContext: any = useContext(NotificationsContext)
    const { secondaryHeading } = contentText ?? {}

    /**
     * Client-side submission handler
     */
    const handleSubmit = async (formData: FormData): Promise<FormErrors> => {
        try {
            await services.postSiteUserInvite({
                user,
                body: formData as any,
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
        <LayoutColumnContainer>
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
                                    cancelButton: 'Discard invite',
                                },
                            }}
                            submitAction={handleSubmit}
                            cancelHref={routes.adminUsersRoot}
                            shouldClearOnSubmitSuccess={true}
                            bodyClassName="u-mb-14 u-p-4 tablet:u-px-14 tablet:u-pt-12 u-pb-8 u-bg-theme-1"
                        >
                            <h2 className="nhsuk-heading-l">
                                {secondaryHeading}
                            </h2>
                        </FormWithErrorSummary>
                    </LayoutColumn>
                </LayoutColumnContainer>
            </LayoutColumn>
        </LayoutColumnContainer>
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
            routeId: '1324ce48-a906-4195-86f6-64c7d33c4191',
        },
        [withUser, withRoutes, withTextContent],
        async (context: GetServerSidePropsContext) => {
            /**
             * Get data from request context
             */
            const props: Partial<Props> = selectPageProps(context)
            const formData: ServerSideFormData = selectFormData(context)
            const requestMethod: requestMethods = selectRequestMethod(context)

            props.forms = {
                [formTypes.INVITE_USER]: {},
            }

            props.layoutId = layoutIds.ADMIN

            /**
             * Return page not found if user doesn't have permissions to invite a user
             */
            if (
                !props.actions?.includes(actionConstants.SITE_ADMIN_MEMBERS_ADD)
            ) {
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
export default AdminUsersInvitePage
