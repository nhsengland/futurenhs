import { GetServerSideProps } from 'next'
import { useState } from 'react'
import { useRouter } from 'next/router'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { layoutIds } from '@constants/routes'
import { requestMethods } from '@constants/fetch'
import { actions as actionConstants } from '@constants/actions'
import { withUser } from '@helpers/hofs/withUser'
import { withRoutes } from '@helpers/hofs/withRoutes'
import { withTextContent } from '@helpers/hofs/withTextContent'
import {
    selectCsrfToken,
    selectMultiPartFormData,
    selectRequestMethod,
    selectUser,
    selectPageProps,
} from '@helpers/selectors/context'
import { postGroup } from '@services/postGroup'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { FormConfig, FormErrors } from '@appTypes/form'
import { formTypes } from '@constants/forms'
import { getGenericFormError } from '@helpers/util/form'
import { FormWithErrorSummary } from '@components/forms/FormWithErrorSummary'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { useFormConfig } from '@helpers/hooks/useForm'
import { GroupPage } from '@appTypes/page'

export interface Props extends GroupPage {
    folderId: string
}

/**
 * Admin create group template
 */
export const AdminCreateGroupPage: (props: Props) => JSX.Element = ({
    csrfToken,
    forms,
    routes,
    user,
    contentText,
    services = {
        postGroup: postGroup,
    },
}) => {
    const router = useRouter()

    const formConfig: FormConfig = useFormConfig(
        formTypes.CREATE_GROUP,
        forms[formTypes.CREATE_GROUP]
    )
    const [errors, setErrors] = useState(formConfig.errors)

    const { secondaryHeading } = contentText ?? {}

    /**
     * Client-side submission handler
     */
    const handleSubmit = async (formData: FormData): Promise<FormErrors> => {
        const headers =
                getStandardServiceHeaders({
                    csrfToken,
                    accessToken: user.accessToken,
                })
        try {
            await services.postGroup({ user, headers, body: formData as any })

            router.replace(`${routes.adminGroupsRoot}`)

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
                                    submitButton: 'Save and create group',
                                    cancelButton: 'Discard group',
                                },
                            }}
                            context={{
                                user: user,
                            }}
                            submitAction={handleSubmit}
                            cancelHref={routes.adminGroupsRoot}
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
            routeId: '3436b6a3-6cb0-4b76-982d-dfc0c487bc52',
        },
        [withUser, withRoutes, withTextContent],
        async (context: GetServerSidePropsContext) => {
            /**
             * Get data from request context
             */
            const props: Partial<Props> = selectPageProps(context)
            const user: User = selectUser(context)
            const csrfToken: string = selectCsrfToken(context)
            const formData: FormData = selectMultiPartFormData(context)
            const requestMethod: string = selectRequestMethod(context)

            props.forms = {
                [formTypes.CREATE_GROUP]: {},
            }
            const form: any = props.forms[formTypes.CREATE_GROUP]

            /**
             * Ticks checkbox by default
             */
            form.initialValues = {
                isPublic: true,
            }

            props.layoutId = layoutIds.ADMIN

            if (
                !props.actions?.includes(actionConstants.SITE_ADMIN_GROUPS_ADD)
            ) {
                return {
                    notFound: true,
                }
            }

            /**
             * handle server-side form POST
             */
            if (formData && requestMethod === requestMethods.POST) {
                const headers = getStandardServiceHeaders({
                    csrfToken,
                    accessToken: user.accessToken,
                })

                try {
                    await postGroup({
                        user,
                        headers,
                        body: formData,
                    })

                    return {
                        redirect: {
                            permanent: false,
                            destination: `${props.routes.adminGroupsRoot}`,
                        },
                    }
                } catch (error) {
                    const validationErrors: FormErrors =
                        getServiceErrorDataValidationErrors(error)

                    if (validationErrors) {
                        form.errors = validationErrors
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
export default AdminCreateGroupPage
