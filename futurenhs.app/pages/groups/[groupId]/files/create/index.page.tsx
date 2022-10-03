import { GetServerSideProps } from 'next'
import { useState } from 'react'
import { useRouter } from 'next/router'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { layoutIds, groupTabIds } from '@constants/routes'
import { requestMethods } from '@constants/fetch'
import { routeParams } from '@constants/routes'
import { actions as actionConstants } from '@constants/actions'
import { withUser } from '@helpers/hofs/withUser'
import { withRoutes } from '@helpers/hofs/withRoutes'
import { withGroup } from '@helpers/hofs/withGroup'
import {
    selectCsrfToken,
    selectMultiPartFormData,
    selectParam,
    selectUser,
    selectQuery,
    selectRequestMethod,
    selectPageProps,
} from '@helpers/selectors/context'
import { postGroupFile } from '@services/postGroupFile'
import { getGroupFolder } from '@services/getGroupFolder'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { withTextContent } from '@helpers/hofs/withTextContent'
import { formTypes } from '@constants/forms'
import { getGenericFormError } from '@helpers/util/form'
import { FormWithErrorSummary } from '@components/forms/FormWithErrorSummary'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { RichText } from '@components/generic/RichText'
import { FormErrors, FormConfig } from '@appTypes/form'
import { useFormConfig } from '@helpers/hooks/useForm'
import { Folder } from '@appTypes/file'
import { GroupPage } from '@appTypes/page'

export interface Props extends GroupPage {
    folderId: string
    folder?: Folder
}

/**
 * Group create file template
 */
export const GroupCreateFilePage: (props: Props) => JSX.Element = ({
    groupId,
    csrfToken,
    forms,
    routes,
    user,
    folderId,
    folder,
    contentText,
}) => {
    const router = useRouter()

    const formConfig: FormConfig = useFormConfig(
        formTypes.CREATE_FILE,
        forms[formTypes.CREATE_FILE]
    )
    const [errors, setErrors] = useState(formConfig?.errors)

    const { text } = folder ?? {}
    const { name } = text ?? {}
    const { secondaryHeading } = contentText ?? {}

    const folderHref: string = `${routes.groupFoldersRoot}/${folderId}`

    const handleSubmit = async (formData: FormData): Promise<FormErrors> => {
        return new Promise((resolve) => {
            postGroupFile({ groupId, folderId, user, body: formData })
                .then(() => {
                    router.push(folderHref)
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
                                    submitButton: 'Upload File',
                                    cancelButton: 'Discard File',
                                },
                            }}
                            submitAction={handleSubmit}
                            cancelHref={folderHref}
                            bodyClassName=""
                            submitButtonClassName="u-float-right"
                        >
                            <h2 className="nhsuk-heading-l u-mb-6 o-truncated-text-lines-1">
                                {name}
                            </h2>
                            <hr />
                            <h3 className="u-mt-6">{secondaryHeading}</h3>
                            <RichText
                                wrapperElementType="div"
                                className="u-mb-10"
                                bodyHtml="<p>Guidance on maximum file size, supported file formats, making sure they are not uploading any sensitive data, etc.</p><p>All uploaded content must conform to the platform's terms and conditions. 
Open terms and conditions in a new window.</p>"
                            />
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
            routeId: '2ff0717e-494f-4400-8c33-600c080e27b7',
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
            const formData: any = selectMultiPartFormData(context)
            const requestMethod: requestMethods = selectRequestMethod(context)

            props.forms = {
                [formTypes.CREATE_FILE]: {},
            }

            props.layoutId = layoutIds.GROUP
            props.tabId = groupTabIds.FILES
            props.pageTitle = `${props.entityText.title} - ${props.contentText.subTitle}`

            if (
                !props.actions?.includes(actionConstants.GROUPS_FILES_ADD) ||
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
                if (formData && requestMethod === requestMethods.POST) {
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
                    props.forms[formTypes.CREATE_FILE].errors = validationErrors
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
export default GroupCreateFilePage
