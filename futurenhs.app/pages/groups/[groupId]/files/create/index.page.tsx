import { GetServerSideProps } from 'next'

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { layoutIds, groupTabIds } from '@constants/routes'
import { requestMethods } from '@constants/fetch'
import { routeParams } from '@constants/routes'
import { actions as actionConstants } from '@constants/actions'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withGroup } from '@hofs/withGroup'
import { withForms } from '@hofs/withForms'
import {
    selectCsrfToken,
    selectMultiPartFormData,
    selectParam,
    selectUser,
    selectQuery,
    selectRequestMethod,
} from '@selectors/context'
import { postGroupFile } from '@services/postGroupFile'
import { getGroupFolder } from '@services/getGroupFolder'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'

import { createFileForm } from '@formConfigs/create-file'
import { GroupCreateFileTemplate } from '@components/_pageTemplates/GroupCreateFileTemplate'
import { Props } from '@components/_pageTemplates/GroupCreateFileTemplate/interfaces'
import { withTextContent } from '@hofs/withTextContent'
import { FormErrors } from '@appTypes/form'

const routeId: string = '2ff0717e-494f-4400-8c33-600c080e27b7'
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
            routeId,
            getServerSideProps: withForms({
                props,
                routeId,
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
                        const folderId: string = selectQuery(
                            context,
                            routeParams.FOLDERID
                        )
                        const csrfToken: string = selectCsrfToken(context)
                        const formData: any = selectMultiPartFormData(context)
                        const requestMethod: requestMethods =
                            selectRequestMethod(context)

                        const form: any = props.forms[createFileForm.id]

                        props.layoutId = layoutIds.GROUP
                        props.tabId = groupTabIds.FILES
                        props.pageTitle = `${props.entityText.title} - ${props.contentText.subTitle}`

                        if (
                            !props.actions?.includes(
                                actionConstants.GROUPS_FILES_ADD
                            ) ||
                            !folderId
                        ) {
                            return {
                                notFound: true,
                            }
                        }

                        /**
                         * Get data from services
                         */
                        try {
                            const [groupFolder] = await Promise.all([
                                getGroupFolder({ user, groupId, folderId }),
                            ])

                            props.folderId = folderId
                            props.folder = groupFolder.data

                            /**
                             * handle server-side form POST
                             */
                            if (
                                formData &&
                                requestMethod === requestMethods.POST
                            ) {
                                const headers = {
                                    ...getStandardServiceHeaders({ csrfToken }),
                                    ...formData.getHeaders(),
                                }

                                await postGroupFile({
                                    groupId,
                                    folderId,
                                    user,
                                    headers,
                                    body: formData,
                                })

                                return {
                                    redirect: {
                                        permanent: false,
                                        destination: `${props.routes.groupFoldersRoot}/${folderId}`,
                                    },
                                }
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
export default GroupCreateFileTemplate
