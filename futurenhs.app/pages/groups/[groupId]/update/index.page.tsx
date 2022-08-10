import { GetServerSideProps } from 'next'

import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { routeParams } from '@constants/routes'
import { requestMethods } from '@constants/fetch'
import { formTypes } from '@constants/forms'
import { actions } from '@constants/actions'
import { layoutIds, groupTabIds } from '@constants/routes'
import { themes, defaultThemeId } from '@constants/themes'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withTextContent } from '@hofs/withTextContent'
import { withGroup } from '@hofs/withGroup'
import {
    selectFormData,
    selectMultiPartFormData,
    selectCsrfToken,
    selectParam,
    selectUser,
    selectRequestMethod,
    selectPageProps
} from '@selectors/context'
import { getGroup } from '@services/getGroup'
import { putGroup } from '@services/putGroup'
import { GetServerSidePropsContext } from '@appTypes/next'
import { FormErrors } from '@appTypes/form'

import { GroupUpdateTemplate } from '@components/_pageTemplates/GroupUpdateTemplate'
import { User } from '@appTypes/user'

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (context: GetServerSidePropsContext) => await pipeSSRProps(context, {
    routeId: '578dfcc6-857f-4eda-8779-1d9b110888c7'
}, [
    withUser,
    withRoutes,
    withGroup,
    withTextContent
], async (context: GetServerSidePropsContext) => {

    /**
     * Get data from request context
     */
    const props: Partial<any> = selectPageProps(context);
    const csrfToken: string = selectCsrfToken(context)
    const currentValues: any = selectFormData(context)
    const submission: any = selectMultiPartFormData(context)
    const groupId: string = selectParam(
        context,
        routeParams.GROUPID
    )
    const user: User = selectUser(context)
    const requestMethod: requestMethods =
        selectRequestMethod(context)

    const formId: string = formTypes.UPDATE_GROUP

    props.csrfToken = csrfToken
    props.layoutId = layoutIds.GROUP
    props.tabId = groupTabIds.INDEX
    props.pageTitle = `${props.entityText.title} - ${props.contentText.subTitle}`

    /**
     * Return not found if user does not have edit group action
     */
    if (!props.actions.includes(actions.GROUPS_EDIT)) {
        return {
            notFound: true,
        }
    }

    /**
     * Get data from services
     */
    try {
        const [group] = await Promise.all([
            getGroup({ user, groupId, isForUpdate: true }),
        ])
        const etag = group.headers.get('etag')

        props.etag = etag
        props.isPublic = group.data.isPublic
        props.forms = {
            [formId]: {
                initialValues: {
                    Name: group.data.text.title,
                    Strapline: group.data.text.strapLine,
                    ImageId: group.data.imageId,
                    ThemeId:
                        group.data.themeId &&
                            themes[group.data.themeId]
                            ? [group.data.themeId]
                            : [defaultThemeId],
                    isPublic: [group.data.isPublic],
                },
            },
        }

        /**
         * Handle server-side form post
         */
        if (
            submission &&
            requestMethod === requestMethods.POST
        ) {
            props.forms[formId].initialValues =
                currentValues.getAll()

            const headers = {
                ...getStandardServiceHeaders({
                    csrfToken,
                    etag,
                }),
                ...submission.getHeaders(),
            }

            await putGroup({
                groupId,
                user,
                headers,
                body: submission,
            })

            return {
                redirect: {
                    permanent: false,
                    destination: props.routes.groupRoot,
                },
            }
        }
    } catch (error: any) {
        const validationErrors: FormErrors =
            getServiceErrorDataValidationErrors(error)

        if (validationErrors) {
            props.forms[formId].errors = validationErrors
        } else {
            return handleSSRErrorProps({ props, error })
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
export default GroupUpdateTemplate
