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
import { postDomain } from '@services/postDomain'

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
                                                                    }) => {
    const formConfig: FormConfig = useFormConfig(
        formTypes.ADD_DOMAIN,
        forms[formTypes.ADD_DOMAIN]
    )
    const [errors, setErrors] = useState(formConfig?.errors)
    const notificationsContext: any = useContext(NotificationsContext)
    const { secondaryHeading } = contentText ?? {}

    /**
     * Client-side submission handler
     */
    const handleSubmit = async (formData: FormData): Promise<FormErrors> => {
        try {
            const domain: FormDataEntryValue = formData.get('Domain')
            const res = await postDomain({
                domain: domain.toString(),
                user,
            })
            useNotification({
                notificationsContext,
                text: {
                    heading: notifications.SUCCESS,
                    body: `Added ${domain} to the domain allow list.`,
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
    console.log(formConfig)
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
                                    submitButton: 'Allow domain',
                                    cancelButton: 'Discard domain',
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
            routeId: '466751c3-f926-44f5-9a3e-137e558f69b7',
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
                [formTypes.ADD_DOMAIN]: {},
            }

            props.layoutId = layoutIds.ADMIN

            /**
             * Return page not found if user doesn't have permissions to invite a user
             */
            if (
                !props.actions?.includes(actionConstants.SITE_ADMIN_DOMAINS_ADD)
            ) {
                return {
                    notFound: true,
                }
            }

            /**
             * Handle server-side form post
             */
            if (formData && requestMethod === requestMethods.POST) {
                props.forms[formTypes.ADD_DOMAIN].initialValues = formData

                try {
                    return {
                        props: props,
                    }
                } catch (error) {
                    const validationErrors: FormErrors =
                        getServiceErrorDataValidationErrors(error)

                    if (validationErrors) {
                        props.forms[formTypes.ADD_DOMAIN].errors =
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
