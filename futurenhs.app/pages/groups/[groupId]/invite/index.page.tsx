import { GetServerSideProps } from 'next'

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'

import { actions as actionConstants } from '@constants/actions'
import { groupTabIds, layoutIds, routeParams } from '@constants/routes'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withTokens } from '@hofs/withTokens'

import { GetServerSidePropsContext } from '@appTypes/next'

import { GroupMemberInviteTemplate } from '@components/_pageTemplates/GroupMemberInviteTemplate'
import { Props } from '@components/_pageTemplates/GroupCreateDiscussionTemplate/interfaces'
import { withTextContent } from '@hofs/withTextContent'
import { withGroup } from '@hofs/withGroup'
import { formTypes } from '@constants/forms'
import {
    selectCsrfToken,
    selectFormData,
    selectParam,
    selectRequestMethod,
    selectUser,
} from '@selectors/context'
import { User } from '@appTypes/user'
import { ServerSideFormData } from '@helpers/util/form'
import { requestMethods } from '@constants/fetch'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { postGroupMemberInvite } from '@services/postGroupMemberInvite'
import { FormErrors } from '@appTypes/form'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'

const routeId: string = 'f872b71a-0449-4821-a8da-b75bbd451b2d'
const props: Partial<Props> = {}

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withUser({
    props,
    getServerSideProps: withRoutes({
        props,
        getServerSideProps: withGroup({
            props,
            getServerSideProps: withTokens({
                props,
                routeId,
                getServerSideProps: withTextContent({
                    props,
                    routeId,
                    getServerSideProps: async (
                        context: GetServerSidePropsContext
                    ) => {
                        const user: User = selectUser(context)
                        const csrfToken: string = selectCsrfToken(context)
                        const formData: ServerSideFormData =
                            selectFormData(context)
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
                        // if (
                        //     !props.actions?.includes(
                        //         actionConstants.GROUPS_MEMBERS_INVITE
                        //     )
                        // ) {
                        //     return {
                        //         notFound: true,
                        //     }
                        // }

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
                        return handleSSRSuccessProps({ props })
                    },
                }),
            }),
        }),
    }),
})

/**
 * Export page template
 */
export default GroupMemberInviteTemplate
