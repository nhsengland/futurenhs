import { GetServerSideProps } from 'next'
import { useState } from 'react'
import { useRouter } from 'next/router'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { actions as actionConstants } from '@constants/actions'
import { routeParams, layoutIds, groupTabIds } from '@constants/routes'
import {
    selectParam,
    selectCsrfToken,
    selectRequestMethod,
} from '@helpers/selectors/context'
import { withUser } from '@helpers/hofs/withUser'
import { withRoutes } from '@helpers/hofs/withRoutes'
import { withGroup } from '@helpers/hofs/withGroup'
import { selectPageProps } from '@helpers/selectors/context'
import { requestMethods } from '@constants/fetch'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { deleteGroupMembership } from '@services/deleteGroupMembership'
import { GetServerSidePropsContext } from '@appTypes/next'
import { useFormConfig } from '@helpers/hooks/useForm'
import { formTypes } from '@constants/forms'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { FormWithErrorSummary } from '@components/forms/FormWithErrorSummary'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { putGroup } from '@services/putGroup'
import { FormErrors, FormConfig, FormField } from '@appTypes/form'
import { getFormField, getGenericFormError } from '@helpers/util/form'
import { Dialog } from '@components/generic/Dialog'
import { GroupPage } from '@appTypes/page'

export interface Props extends GroupPage {
    folderId: string
}

/**
 * Group create folder template
 */
export const GroupUpdatePage: (props: Props) => JSX.Element = ({
    groupId,
    user,
    csrfToken,
    etag,
    forms,
    routes,
    contentText,
    isPublic,
    services = {
        putGroup: putGroup,
    },
}) => {
    const router = useRouter()
    const [isChangeGroupPrivacyModalOpen, setIsChangeGroupPrivacyModalOpen] =
        useState(false)
    const [groupEditFormData, setGroupEditFormData] = useState({})

    const formConfig: FormConfig = useFormConfig(
        formTypes.UPDATE_GROUP,
        forms[formTypes.UPDATE_GROUP]
    )
    const [errors, setErrors] = useState(formConfig?.errors)

    const groupPrivacyField: FormField = getFormField(formConfig, 'isPublic')

    /**
     * Hides group is public checkbox if group is already private
     */
    if (!isPublic && groupPrivacyField) {
        groupPrivacyField.shouldRender = false
    }

    const { mainHeading, secondaryHeading } = contentText ?? {}

    /**
     * Handle client-side update submission
     */
    const handleSubmit = async (formData: FormData): Promise<FormErrors> => {
        /**
         * If checkbox is unticked, store the form data and reveal confirmation dialog. Submission is then handled by the dialog confirm.
         */
        setGroupEditFormData(formData)
        const isPublicBoxTicked: boolean = Boolean(formData.get('isPublic'))

        if (
            !isPublicBoxTicked &&
            !isChangeGroupPrivacyModalOpen &&
            groupPrivacyField.shouldRender !== false
        ) {
            setIsChangeGroupPrivacyModalOpen(true)
            return
        }

        return new Promise((resolve) => {
            const headers = getStandardServiceHeaders({ csrfToken, etag })

            services
                .putGroup({ groupId, user, headers, body: formData })
                .then(() => {
                    setIsChangeGroupPrivacyModalOpen(false)
                    setErrors({})
                    resolve({})

                    router.replace(routes.groupRoot)

                    /**
                     * Full page reload currently necessary to clear image cache of previous group image
                     */
                    window.location.replace(routes.groupRoot)
                })
                .catch((error) => {
                    const errors: FormErrors =
                        getServiceErrorDataValidationErrors(error) ||
                        getGenericFormError(error)

                    setIsChangeGroupPrivacyModalOpen(false)
                    setErrors(errors)
                    resolve(errors)
                })
        })
    }

    const handleSubmitConfirm = (): void => {
        handleSubmit(groupEditFormData as any)
    }

    const handleSubmitCancel = (): void => {
        setIsChangeGroupPrivacyModalOpen(false)
    }

    /**
     * Render
     */
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
                                    submitButton: 'Save and close',
                                    cancelButton: 'Discard changes',
                                },
                            }}
                            cancelHref={routes.groupRoot}
                            submitAction={handleSubmit}
                            bodyClassName="u-mb-12"
                        >
                            <h2 className="nhsuk-heading-l">{mainHeading}</h2>
                            <p className="u-text-lead u-text-theme-7 u-mb-4">
                                {secondaryHeading}
                            </p>
                        </FormWithErrorSummary>
                    </LayoutColumn>
                </LayoutColumnContainer>
                <Dialog
                    id="dialog-change-group-privacy"
                    isOpen={isChangeGroupPrivacyModalOpen}
                    text={{
                        cancelButton: 'Cancel',
                        confirmButton: 'Yes, submit',
                        heading: 'Change group privacy',
                    }}
                    cancelAction={handleSubmitCancel}
                    confirmAction={handleSubmitConfirm}
                >
                    <p className="u-text-bold">
                        Unselecting 'Group is public?' will set this group to
                        private, restricting access to approved members only.
                        This cannot be undone, are you sure you wish to
                        continue?
                    </p>
                </Dialog>
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
            const requestMethod: requestMethods = selectRequestMethod(context)
            const csrfToken: string = selectCsrfToken(context)
            const groupId: string = selectParam(context, routeParams.GROUPID)

            props.layoutId = layoutIds.GROUP
            props.tabId = groupTabIds.INDEX

            /**
             * Return not found if user does not have valid action to leave group
             */
            if (!props.actions?.includes(actionConstants.GROUPS_LEAVE)) {
                return {
                    notFound: true,
                }
            }

            /**
             * Return error if request is not a POST
             */
            if (requestMethod !== requestMethods.POST) {
                return handleSSRErrorProps({
                    props,
                    error: new Error('405 Method Not Allowed'),
                })
            }

            /**
             * Get data from services
             */
            try {
                await deleteGroupMembership({
                    groupId,
                    csrfToken,
                    user: props.user,
                })

                /**
                 * Return data to page template
                 */
                return {
                    redirect: {
                        permanent: false,
                        destination: props.routes.groupRoot,
                    },
                }
            } catch (error) {
                return handleSSRErrorProps({ props, error })
            }
        }
    )

/**
 * Export page template
 */
export default GroupUpdatePage
