import { GetServerSideProps } from 'next'

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { layoutIds, groupTabIds } from '@constants/routes'
import { formTypes } from '@constants/forms'
import { routeParams } from '@constants/routes'
import { requestMethods } from '@constants/fetch'
import { actions as actionConstants } from '@constants/actions'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withGroup } from '@hofs/withGroup'
import {
    selectCsrfToken,
    selectFormData,
    selectParam,
    selectUser,
    selectRequestMethod,
} from '@selectors/context'
import { putGroupFolder } from '@services/putGroupFolder'
import { getGroupFolder } from '@services/getGroupFolder'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { FormErrors } from '@appTypes/form'

import { GroupCreateUpdateFolderTemplate } from '@components/_pageTemplates/GroupCreateUpdateFolderTemplate'
import { Props } from '@components/_pageTemplates/GroupCreateUpdateFolderTemplate/interfaces'
import { withTextContent } from '@hofs/withTextContent'
import { ServerSideFormData } from '@helpers/util/form'

const routeId: string = 'cd828945-f799-40e9-be00-64e76809e00d'
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
                    const folderId: string = selectParam(
                        context,
                        routeParams.FOLDERID
                    )
                    const csrfToken: string = selectCsrfToken(context)
                    const formData: ServerSideFormData = selectFormData(context)
                    const requestMethod: string = selectRequestMethod(context)

                    props.layoutId = layoutIds.GROUP
                    props.tabId = groupTabIds.FILES
                    props.folderId = folderId
                    props.pageTitle = `${props.entityText.title} - ${props.contentText.subTitle}`

                    /**
                     * Return not found if user does not have folder edit action
                     */
                    if (
                        !props.actions?.includes(
                            actionConstants.GROUPS_FOLDERS_EDIT
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
                                getGroupFolder({
                                    user,
                                    groupId,
                                    folderId,
                                    isForUpdate: true,
                                }),
                            ])
                            const etag = groupFolder.headers.get('etag')

                            props.etag = etag
                            props.folder = groupFolder.data

                            props.forms = {
                                [formTypes.GROUP_FOLDER]: {
                                    initialValues: {
                                        Name: props.folder?.text?.name,
                                        Description: props.folder?.text?.body,
                                    },
                                },
                            }

                            /**
                             * handle server-side form POST
                             */
                            if (
                                formData &&
                                requestMethod === requestMethods.POST
                            ) {
                                props.forms[
                                    formTypes.GROUP_FOLDER
                                ].initialValues = formData.getAll()

                                const headers = getStandardServiceHeaders({
                                    csrfToken,
                                    etag,
                                })

                                await putGroupFolder({
                                    groupId,
                                    folderId,
                                    user,
                                    headers,
                                    body: formData,
                                })

                                return {
                                    redirect: {
                                        permanent: false,
                                        destination: props.routes.groupFolder,
                                    },
                                }
                            }
                        } catch (error) {
                            const validationErrors: FormErrors =
                                getServiceErrorDataValidationErrors(error)

                            if (validationErrors) {
                                props.forms[formTypes.GROUP_FOLDER].errors =
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
                },
            }),
        }),
    }),
})

/**
 * Export page template
 */
export default GroupCreateUpdateFolderTemplate
