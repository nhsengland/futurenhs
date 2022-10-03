import { GetServerSideProps } from 'next'
import { useState } from 'react'
import { useRouter } from 'next/router'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { layoutIds, groupTabIds } from '@constants/routes'
import { routeParams } from '@constants/routes'
import { actions as actionConstants } from '@constants/actions'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withGroup } from '@hofs/withGroup'
import { deleteGroupFolder } from '@services/deleteGroupFolder'
import {
    selectParam,
    selectUser,
    selectQuery,
    selectCsrfToken,
    selectPageProps,
} from '@selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { formTypes } from '@constants/forms'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { getGenericFormError } from '@helpers/util/form'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { FormWithErrorSummary } from '@components/FormWithErrorSummary'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { postGroupFolder } from '@services/postGroupFolder'
import { putGroupFolder } from '@services/putGroupFolder'
import { FormErrors, FormConfig } from '@appTypes/form'
import { useFormConfig } from '@hooks/useForm'
import { Folder } from '@appTypes/file'
import { GroupPage } from '@appTypes/page'

export interface Props extends GroupPage {
    folderId: string
    folder?: Folder
}

/**
 * Group create/update folder template
 */
export const GroupDeletePage: (props: Props) => JSX.Element = ({
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
        {},
        [withUser, withRoutes, withGroup],
        async (context: GetServerSidePropsContext) => {
            /**
             * Get data from request context
             */
            const props: Partial<any> = selectPageProps(context)
            const user: User = selectUser(context)
            const groupId: string = selectParam(context, routeParams.GROUPID)
            const folderId: string = selectQuery(context, routeParams.FOLDERID)
            const csrfToken: string = selectCsrfToken(context)

            props.layoutId = layoutIds.GROUP
            props.tabId = groupTabIds.FILES
            props.folderId = folderId

            /**
             * Return not found if user does not have ability to delete folders in this group
             */
            if (
                !props.actions?.includes(actionConstants.GROUPS_FOLDERS_DELETE)
            ) {
                return {
                    notFound: true,
                }
            }

            /**
             * Attempt to delete group folder
             */
            try {
                await deleteGroupFolder({
                    user,
                    groupId,
                    folderId,
                    csrfToken,
                })

                /**
                 * Redirect to home
                 */
                return {
                    redirect: {
                        permanent: false,
                        destination: props.routes.groupFoldersRoot,
                    },
                }
            } catch (error) {
                return handleSSRErrorProps({ error, props })
            }
        }
    )

/**
 * Export page template
 */
export default GroupDeletePage
