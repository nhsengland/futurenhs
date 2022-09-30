import { GetServerSideProps } from 'next'
import { useState } from 'react'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { routeParams } from '@constants/routes'
import { requestMethods } from '@constants/fetch'
import { formTypes } from '@constants/forms'
import { actions } from '@constants/actions'
import { layoutIds, groupTabIds } from '@constants/routes'
import { themes, defaultThemeId } from '@constants/themes'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withTextContent } from '@hofs/withTextContent'
import { withGroup } from '@hofs/withGroup'
import {
    selectFormData,
    selectMultiPartFormData,
    selectCsrfToken,
    selectParam,
    selectUser,
    selectRequestMethod,
    selectPageProps,
} from '@selectors/context'
import { getGroup } from '@services/getGroup'
import { putGroup } from '@services/putGroup'
import { GetServerSidePropsContext } from '@appTypes/next'
import { FormErrors } from '@appTypes/form'
import { User } from '@appTypes/user'
import { useRouter } from 'next/router'
import { useFormConfig } from '@hooks/useForm'
import { FormWithErrorSummary } from '@components/FormWithErrorSummary'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { FormConfig, FormField } from '@appTypes/form'
import { getFormField, getGenericFormError } from '@helpers/util/form'
import { Dialog } from '@components/Dialog'
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
        {
            routeId: '578dfcc6-857f-4eda-8779-1d9b110888c7',
        },
        [withUser, withRoutes, withGroup, withTextContent],
        async (context: GetServerSidePropsContext) => {
            /**
             * Get data from request context
             */
            const props: Partial<any> = selectPageProps(context)
            const csrfToken: string = selectCsrfToken(context)
            const currentValues: any = selectFormData(context)
            const submission: any = selectMultiPartFormData(context)
            const groupId: string = selectParam(context, routeParams.GROUPID)
            const user: User = selectUser(context)
            const requestMethod: requestMethods = selectRequestMethod(context)

            const formId: string = formTypes.UPDATE_GROUP

            props.csrfToken = csrfToken
            props.layoutId = layoutIds.GROUP
            props.tabId = groupTabIds.INDEX
            props.pageTitle = `${props.entityText.title} - ${props.contentText.subTitle}`

            /**
             * Return not found if user does not have edit group action
             */
            if (!props.actions.includes(actions.GROUPS_EDIT)) {
                return {
                    notFound: true,
                }
            }

            /**
             * Get data from services
             */
            try {
                const [group] = await Promise.all([
                    getGroup({ user, groupId, isForUpdate: true }),
                ])
                const etag = group.headers.get('etag')

                props.etag = etag
                props.isPublic = group.data.isPublic
                props.forms = {
                    [formId]: {
                        initialValues: {
                            Name: group.data.text.title,
                            Strapline: group.data.text.strapLine,
                            ImageId: group.data.imageId,
                            ThemeId:
                                group.data.themeId && themes[group.data.themeId]
                                    ? [group.data.themeId]
                                    : [defaultThemeId],
                            isPublic: [group.data.isPublic],
                        },
                    },
                }

                /**
                 * Handle server-side form post
                 */
                if (submission && requestMethod === requestMethods.POST) {
                    props.forms[formId].initialValues = currentValues.getAll()

                    const headers = {
                        ...getStandardServiceHeaders({
                            csrfToken,
                            etag,
                        }),
                        ...submission.getHeaders(),
                    }

                    await putGroup({
                        groupId,
                        user,
                        headers,
                        body: submission,
                    })

                    return {
                        redirect: {
                            permanent: false,
                            destination: props.routes.groupRoot,
                        },
                    }
                }
            } catch (error: any) {
                const validationErrors: FormErrors =
                    getServiceErrorDataValidationErrors(error)

                if (validationErrors) {
                    props.forms[formId].errors = validationErrors
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
export default GroupUpdatePage
