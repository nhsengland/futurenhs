import { GetServerSideProps } from 'next'

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { routeParams } from '@constants/routes'
import { requestMethods } from '@constants/fetch'
import { layoutIds, groupTabIds } from '@constants/routes'
import { actions as actionConstants } from '@constants/actions'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withGroup } from '@hofs/withGroup'
import {
    selectFormData,
    selectCsrfToken,
    selectParam,
    selectUser,
    selectRequestMethod,
} from '@selectors/context'
import { postGroupDiscussion } from '@services/postGroupDiscussion'
import { formTypes } from '@constants/forms'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { FormErrors } from '@appTypes/form'

import { GroupCreateDiscussionTemplate } from '@components/_pageTemplates/GroupCreateDiscussionTemplate'
import { Props } from '@components/_pageTemplates/GroupCreateDiscussionTemplate/interfaces'
import { withTextContent } from '@hofs/withTextContent'
import { ServerSideFormData } from '@helpers/util/form'

const routeId: string = 'fcf3d540-9a55-418c-b317-a14146ae075f'
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
            getServerSideProps: withTextContent({
                props,
                routeId,
                getServerSideProps: async (
                    context: GetServerSidePropsContext
                ) => {
                    const user: User = selectUser(context)
                    const groupId: string = selectParam(
                        context,
                        routeParams.GROUPID
                    )
                    const csrfToken: string = selectCsrfToken(context)
                    const formData: ServerSideFormData = selectFormData(context)
                    const requestMethod: requestMethods =
                        selectRequestMethod(context)

                    props.forms = {
                        [formTypes.CREATE_DISCUSSION]: {},
                    }

                    props.layoutId = layoutIds.GROUP
                    props.tabId = groupTabIds.FORUM
                    props.pageTitle = `${props.entityText.title} - ${props.contentText.subTitle}`

                    /**
                     * Return page not found if user doesn't have permissions to create a discussion
                     */
                    if (
                        !props.actions?.includes(
                            actionConstants.GROUPS_DISCUSSIONS_ADD
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
                        props.forms[formTypes.CREATE_DISCUSSION].initialValues =
                            formData

                        try {
                            const headers: any = getStandardServiceHeaders({
                                csrfToken,
                            })

                            await postGroupDiscussion({
                                groupId,
                                user,
                                headers,
                                body: formData,
                            })

                            return {
                                redirect: {
                                    permanent: false,
                                    destination: props.routes.groupForumRoot,
                                },
                            }
                        } catch (error) {
                            const validationErrors: FormErrors =
                                getServiceErrorDataValidationErrors(error)

                            if (validationErrors) {
                                props.forms[
                                    formTypes.CREATE_DISCUSSION
                                ].errors = validationErrors
                            } else {
                                return handleSSRErrorProps({ props, error })
                            }
                        }
                    }

                    /**
                     * Return data to page template
                     */
                    return handleSSRSuccessProps({ props, context })
                },
            }),
        }),
    }),
})

/**
 * Export page template
 */
export default GroupCreateDiscussionTemplate
