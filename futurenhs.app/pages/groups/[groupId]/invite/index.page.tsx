import { GetServerSideProps } from 'next'

import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'

import { actions as actionConstants } from '@constants/actions'
import { groupTabIds, layoutIds, routeParams } from '@constants/routes'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'

import { GetServerSidePropsContext } from '@appTypes/next'

import { GroupMemberInviteTemplate } from '@components/_pageTemplates/GroupMemberInviteTemplate'
import { withTextContent } from '@hofs/withTextContent'
import { withGroup } from '@hofs/withGroup'
import { formTypes } from '@constants/forms'
import {
    selectCsrfToken,
    selectFormData,
    selectParam,
    selectRequestMethod,
    selectUser,
    selectPageProps
} from '@selectors/context'
import { User } from '@appTypes/user'
import { ServerSideFormData } from '@helpers/util/form'
import { requestMethods } from '@constants/fetch'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { postGroupMemberInvite } from '@services/postGroupMemberInvite'
import { FormErrors } from '@appTypes/form'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { notifications } from '@constants/notifications'

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (context: GetServerSidePropsContext) => await pipeSSRProps(context, {
    routeId: 'f872b71a-0449-4821-a8da-b75bbd451b2d'
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
    const user: User = selectUser(context)
    const csrfToken: string = selectCsrfToken(context)
    const formData: ServerSideFormData = selectFormData(context)
    const groupId: string = selectParam(
        context,
        routeParams.GROUPID
    )
    const requestMethod: requestMethods =
        selectRequestMethod(context)

    props.layoutId = layoutIds.GROUP
    props.tabId = groupTabIds.MEMBERS
    props.pageTitle = `${props.entityText.title} - ${props.contentText.subTitle}`

    props.forms = {
        [formTypes.INVITE_USER]: {},
    }

    /**
     * Return page not found if user doesn't have permissions to invite a user - TODO: Pending API
     */
    if (
        !props.actions?.includes(
            actionConstants.GROUPS_MEMBERS_INVITE
        )
    ) {
        return {
            notFound: true,
        }
    }

    /**
     * Handle server-side form post
     */
    if (formData && requestMethod === requestMethods.POST) {
        props.forms[formTypes.INVITE_USER].initialValues =
            formData

        try {
            const headers: any = getStandardServiceHeaders({
                csrfToken,
            })

            await postGroupMemberInvite({
                user,
                headers,
                body: formData,
                groupId,
            })

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

});

/**
 * Export page template
 */
export default GroupMemberInviteTemplate
