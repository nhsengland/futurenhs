import { GetServerSideProps } from 'next'
import { useState } from 'react'
import { useRouter } from 'next/router'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { layoutIds, groupTabIds } from '@constants/routes'
import { routeParams } from '@constants/routes'
import { requestMethods } from '@constants/fetch'
import { actions as actionConstants } from '@constants/actions'
import { withUser } from '@helpers/hofs/withUser'
import { withRoutes } from '@helpers/hofs/withRoutes'
import { withGroup } from '@helpers/hofs/withGroup'
import {
    selectCsrfToken,
    selectFormData,
    selectRequestMethod,
    selectParam,
    selectUser,
    selectQuery,
} from '@helpers/selectors/context'
import { getGroupFolder } from '@services/getGroupFolder'
import { selectPageProps } from '@helpers/selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { withTextContent } from '@helpers/hofs/withTextContent'
import { ServerSideFormData } from '@helpers/util/form'
import { formTypes } from '@constants/forms'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { getGenericFormError } from '@helpers/util/form'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { FormWithErrorSummary } from '@components/forms/FormWithErrorSummary'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { postGroupFolder } from '@services/postGroupFolder'
import { putGroupFolder } from '@services/putGroupFolder'
import { FormErrors, FormConfig } from '@appTypes/form'
import { useFormConfig } from '@helpers/hooks/useForm'
import { Folder } from '@appTypes/file'
import { GroupPage } from '@appTypes/page'

export interface Props extends GroupPage {
    folderId: string
    folder?: Folder
}

/**
 * Group create/update folder template
 */
export const GroupCreateFolderPage: (props: Props) => JSX.Element = ({
    etag,
    groupId,
    folderId,
    csrfToken,
    forms,
    routes,
    user,
    folder,
    contentText,
}) => {
    const router = useRouter()

    const formConfig: FormConfig = useFormConfig(
        formTypes.GROUP_FOLDER,
        forms[formTypes.GROUP_FOLDER]
    )
    const [errors, setErrors] = useState(formConfig?.errors)

    const { text } = folder ?? {}
    const { name } = text ?? {}
    const { secondaryHeading } = contentText ?? {}

    const handleSubmit = async (formData: FormData): Promise<FormErrors> => {
        return new Promise((resolve) => {
            const serviceToUse =
                router.asPath.indexOf('/update') > -1
                    ? putGroupFolder
                    : postGroupFolder
            const headers = getStandardServiceHeaders({ csrfToken, etag })

            serviceToUse({ groupId, folderId, user, headers, body: formData })
                .then((folderId: any) => {
                    router.push(
                        folderId
                            ? `${routes.groupFoldersRoot}/${folderId}`
                            : routes.groupFolder || routes.groupFoldersRoot
                    )

                    resolve({})
                })
                .catch((error) => {
                    const errors: FormErrors =
                        getServiceErrorDataValidationErrors(error) ||
                        getGenericFormError(error)

                    setErrors(errors)
                    resolve(errors)
                })
        })
    }

    return (
        <>
            <LayoutColumn className="c-page-body">
                <LayoutColumnContainer>
                    <LayoutColumn tablet={8}>
                        <FormWithErrorSummary
                            csrfToken={csrfToken}
                            formConfig={formConfig}
                            errors={errors}
                            text={{
                                form: {
                                    submitButton: 'Save and continue',
                                    cancelButton: 'Discard',
                                },
                            }}
                            submitAction={handleSubmit}
                            cancelHref={
                                routes.groupFolder || routes.groupFoldersRoot
                            }
                            bodyClassName="u-mb-14 u-p-4 tablet:u-px-14 tablet:u-pt-12 u-pb-8 u-bg-theme-1"
                            submitButtonClassName="u-float-right"
                        >
                            {name && (
                                <>
                                    <h2 className="nhsuk-heading-l o-truncated-text-lines-3">
                                        {name}
                                    </h2>
                                    <hr />
                                </>
                            )}
                            {name ? (
                                <h3 className="nhsuk-heading-m">
                                    {secondaryHeading}
                                </h3>
                            ) : (
                                <h2 className="nhsuk-heading-l">
                                    {secondaryHeading}
                                </h2>
                            )}
                        </FormWithErrorSummary>
                    </LayoutColumn>
                </LayoutColumnContainer>
            </LayoutColumn>
        </>
    )
}

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (
    context: GetServerSidePropsContext
) =>
    await pipeSSRProps(
        context,
        {
            routeId: 'c1bc7b37-762f-4ed8-aed2-79fcd0e5d5d2',
        },
        [withUser, withRoutes, withGroup, withTextContent],
        async (context: GetServerSidePropsContext) => {
            /**
             * Get data from request context
             */
            const props: Partial<any> = selectPageProps(context)
            const user: User = selectUser(context)
            const groupId: string = selectParam(context, routeParams.GROUPID)
            const folderId: string = selectQuery(context, routeParams.FOLDERID)
            const csrfToken: string = selectCsrfToken(context)
            const formData: ServerSideFormData = selectFormData(context)
            const requestMethod: string = selectRequestMethod(context)

            props.forms = {
                [formTypes.GROUP_FOLDER]: {},
            }

            props.layoutId = layoutIds.GROUP
            props.tabId = groupTabIds.FILES
            props.folderId = folderId
            props.folder = null
            props.pageTitle = `${props.entityText.title} - ${props.contentText.subTitle}`

            if (!props.actions?.includes(actionConstants.GROUPS_FOLDERS_ADD)) {
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
                props.forms[formTypes.GROUP_FOLDER].initialValues =
                    formData.getAll()

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
        }
    )

/**
 * Export page template
 */
export default GroupCreateFolderPage
