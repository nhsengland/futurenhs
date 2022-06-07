import { GetServerSideProps } from 'next'

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { layoutIds, groupTabIds } from '@constants/routes'
import { routeParams } from '@constants/routes'
import { requestMethods } from '@constants/fetch'
import { actions as actionConstants } from '@constants/actions'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withGroup } from '@hofs/withGroup'
import { withTokens } from '@hofs/withTokens'
import {
    selectCsrfToken,
    selectFormData,
    selectRequestMethod,
    selectParam,
    selectUser,
    selectQuery,
} from '@selectors/context'
import { postGroupFolder } from '@services/postGroupFolder'
import { getGroupFolder } from '@services/getGroupFolder'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { FormErrors } from '@appTypes/form'

import { formTypes } from '@constants/forms'
import { GroupCreateUpdateFolderTemplate } from '@components/_pageTemplates/GroupCreateUpdateFolderTemplate'
import { Props } from '@components/_pageTemplates/GroupCreateUpdateFolderTemplate/interfaces'
import { withTextContent } from '@hofs/withTextContent'
import { ServerSideFormData } from '@helpers/util/form'

const routeId: string = 'c1bc7b37-762f-4ed8-aed2-79fcd0e5d5d2'
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
                        const groupId: string = selectParam(
                            context,
                            routeParams.GROUPID
                        )
                        const folderId: string = selectQuery(
                            context,
                            routeParams.FOLDERID
                        )
                        const csrfToken: string = selectCsrfToken(context)
                        const formData: ServerSideFormData =
                            selectFormData(context)
                        const requestMethod: string =
                            selectRequestMethod(context)

                        props.forms = {
                            [formTypes.GROUP_FOLDER]: {}
                        }

                        props.layoutId = layoutIds.GROUP
                        props.tabId = groupTabIds.FILES
                        props.folderId = folderId
                        props.folder = null
                        props.pageTitle = `${props.entityText.title} - ${props.contentText.subTitle}`

                        if (
                            !props.actions?.includes(
                                actionConstants.GROUPS_FOLDERS_ADD
                            )
                        ) {
                            return {
                                notFound: true,
                            }
                        }

                        /**
                         * Get data from services
                         */
                        if (folderId) {
                            try {
                                const [groupFolder] = await Promise.all([
                                    getGroupFolder({ user, groupId, folderId }),
                                ])

                                props.folder = groupFolder.data
                            } catch (error) {
                                return handleSSRErrorProps({ props, error })
                            }
                        }

                        /**
                         * handle server-side form POST
                         */
                        if (formData && requestMethod === requestMethods.POST) {
                            props.forms[formTypes.GROUP_FOLDER].initialValues = formData.getAll()

                            const headers = getStandardServiceHeaders({
                                csrfToken,
                            })

                            try {
                                const newFolderId = await postGroupFolder({
                                    groupId,
                                    folderId,
                                    user,
                                    headers,
                                    body: formData,
                                })

                                return {
                                    redirect: {
                                        permanent: false,
                                        destination: newFolderId
                                            ? `${props.routes.groupFoldersRoot}/${newFolderId}`
                                            : props.routes.groupFolder ||
                                              props.routes.groupFoldersRoot,
                                    },
                                }
                            } catch (error) {
                                const validationErrors: FormErrors =
                                    getServiceErrorDataValidationErrors(error)

                                if (validationErrors) {
                                    props.forms[formTypes.GROUP_FOLDER].errors = validationErrors
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
export default GroupCreateUpdateFolderTemplate
