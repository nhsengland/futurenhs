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
import { Props } from '@components/_pageTemplates/SiteUserTemplate/interfaces'
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

const routeId: string = '9e86c5cc-6836-4319-8d9d-b96249d4c909'
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
            routeId: '9e86c5cc-6836-4319-8d9d-b96249d4c909',
        },
        [withUser, withRoutes, withTextContent],
        async (context: GetServerSidePropsContext) => {
            const targetUserId: string = selectParam(
                context,
                routeParams.USERID
            )
            const user: User = selectUser(context)

            const isAdmin = props.actions.includes(
                actions.SITE_ADMIN_MEMBERS_EDIT
            )
            const hasCreatePermissions = isAdmin || targetUserId === user.id

            if (!hasCreatePermissions) {
                return {
                    notFound: true,
                }
            }

            const csrfToken: string = selectCsrfToken(context)
            const currentValues: any = selectFormData(context)
            const submission: any = selectMultiPartFormData(context)
            const requestMethod: requestMethods = selectRequestMethod(context)

            props.layoutId = layoutIds.ADMIN
            props.forms = {
                [formTypes.UPDATE_SITE_USER]: {},
            }

            const profileForm: FormConfig =
                props.forms[formTypes.UPDATE_SITE_USER]

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
                        formTypes.UPDATE_SITE_USER
                    ) {
                        profileForm.initialValues = currentValues.getAll()

                        const headers = {
                            ...getStandardServiceHeaders({
                                csrfToken,
                            }),
                            ...submission.getHeaders(),
                        }

                        await putSiteUser({
                            headers,
                            user,
                            body: submission,
                            targetUserId,
                        })
                    }

                    return {
                        redirect: {
                            permanent: false,
                            destination: `${props.routes.usersRoot}/${targetUserId}`,
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
export default () => (
    <div className="c-page-body u-w-full tablet:u-px-4 u-py-10 tablet:u-px-0">
        <h1>Create user profile</h1>
    </div>
)
