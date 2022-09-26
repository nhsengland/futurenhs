import { GetServerSideProps } from 'next'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { layoutIds, routeParams } from '@constants/routes'
import { withUser } from '@hofs/withUser'
import { withTextContent } from '@hofs/withTextContent'
import { withRoutes } from '@hofs/withRoutes'
import {
    selectCsrfToken,
    selectFormData,
    selectMultiPartFormData,
    selectPageProps,
    selectParam,
    selectRequestMethod,
    selectUser,
} from '@selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { getSiteUser } from '@services/getSiteUser'
import { getSiteUserRole } from '@services/getSiteUserRole'
import { formTypes } from '@constants/forms'
import { FormConfig, FormOptions } from '@appTypes/form'
import { setFormConfigOptions } from '@helpers/util/form'
import { Props } from '@components/_pageTemplates/AuthRegisterTemplate/interfaces'
import { withTokens } from '@hofs/withTokens'
import { requestMethods } from '@constants/fetch'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { putSiteUser } from '@services/putSiteUser'
import { FormErrors } from '@appTypes/form'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { User } from '@appTypes/user'
import { SiteUserUpdateTemplate } from '@components/_pageTemplates/SiteUserUpdateTemplate'
import { actions } from '@constants/actions'
import { putSiteUserRole } from '@services/putSiteUserRole'
import { getSiteUserRoles } from '@services/getSiteUserRoles'
import { selectForm } from '@selectors/forms'
import formConfigs from '@formConfigs/index'
import { postRegisterSiteUser } from '@services/postRegisterSiteUser'
import { AuthRegisterTemplate } from '@components/_pageTemplates/AuthRegisterTemplate'
import { getSession } from 'next-auth/react'

const props: Partial<Props> = {}

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (
    context: GetServerSidePropsContext
) =>
    await pipeSSRProps(
        context,
        {
            routeId: 'a49b36cd-3423-4d5d-ae59-85a86b5a1d65',
        },
        [withRoutes, withTextContent],
        async (context: GetServerSidePropsContext) => {
            const props: Partial<Props> = selectPageProps(context)
            const csrfToken: string = selectCsrfToken(context)
            const currentValues: any = selectFormData(context)
            const submission: any = selectMultiPartFormData(context)
            const requestMethod: requestMethods = selectRequestMethod(context)
            const session = await getSession(context)
            props.subjectId = session.sub as string
            props.emailAddress = session.user?.email
            props.issuer = session.iss as string

            /**
             * Hide breadcrumbs
             */
            ;(props as any).breadCrumbList = []

            props.forms = {
                [formTypes.REGISTER_SITE_USER]: {},
            }

            const profileForm: FormConfig =
                props.forms[formTypes.REGISTER_SITE_USER]

            /**
             * Get data from services
             */
            try {
                /**
                 * Handle server-side profile form post
                 */
                if (submission && requestMethod === requestMethods.POST) {
                    if (
                        currentValues.body?.['_form-id'] ===
                        formTypes.REGISTER_SITE_USER
                    ) {
                        profileForm.initialValues = currentValues.getAll()

                        const headers = {
                            ...getStandardServiceHeaders({
                                csrfToken,
                            }),
                            ...submission.getHeaders(),
                        }

                        await postRegisterSiteUser({
                            headers,
                            body: submission,
                            subjectId: session.sub as string,
                            emailAddress: session.user?.email,
                            issuer: session.iss as string,
                        })
                    }

                    return {
                        redirect: {
                            permanent: false,
                            destination: `auth/register`,
                        },
                    }
                }
            } catch (error: any) {
                const validationErrors: FormErrors =
                    getServiceErrorDataValidationErrors(error)

                if (validationErrors) {
                    profileForm.errors = validationErrors
                } else {
                    return handleSSRErrorProps({ props, error })
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
export default AuthRegisterTemplate
