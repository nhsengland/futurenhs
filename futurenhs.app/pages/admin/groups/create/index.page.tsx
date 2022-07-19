import { GetServerSideProps } from 'next'

import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { layoutIds } from '@constants/routes'
import { requestMethods } from '@constants/fetch'
import { actions as actionConstants } from '@constants/actions'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withTextContent } from '@hofs/withTextContent'
import {
    selectCsrfToken,
    selectMultiPartFormData,
    selectRequestMethod,
    selectUser,
    selectPageProps
} from '@selectors/context'
import { postGroup } from '@services/postGroup'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { FormErrors } from '@appTypes/form'

import { AdminCreateGroupTemplate } from '@components/_pageTemplates/AdminCreateGroupTemplate'
import { Props } from '@components/_pageTemplates/AdminCreateGroupTemplate/interfaces'
import { formTypes } from '@constants/forms'

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (context: GetServerSidePropsContext) => await pipeSSRProps(context, {
    routeId: '3436b6a3-6cb0-4b76-982d-dfc0c487bc52'
}, [
    withUser,
    withRoutes,
    withTextContent
], async (context: GetServerSidePropsContext) => {

    /**
     * Get data from request context
     */
    const props: Partial<Props> = selectPageProps(context);
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
        !props.actions?.includes(
            actionConstants.SITE_ADMIN_GROUPS_ADD
        )
    ) {
        return {
            notFound: true,
        }
    }

    /**
     * handle server-side form POST
     */
    if (formData && requestMethod === requestMethods.POST) {
        const headers = getStandardServiceHeaders({ csrfToken })

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

});

/**
 * Export page template
 */
export default AdminCreateGroupTemplate
