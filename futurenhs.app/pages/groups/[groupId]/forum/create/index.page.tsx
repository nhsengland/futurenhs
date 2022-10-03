import { GetServerSideProps } from 'next'
import { useState } from 'react'
import { useRouter } from 'next/router'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { routeParams } from '@constants/routes'
import { requestMethods } from '@constants/fetch'
import { layoutIds, groupTabIds } from '@constants/routes'
import { actions as actionConstants } from '@constants/actions'
import { withUser } from '@helpers/hofs/withUser'
import { withRoutes } from '@helpers/hofs/withRoutes'
import { withGroup } from '@helpers/hofs/withGroup'
import {
    selectFormData,
    selectCsrfToken,
    selectParam,
    selectUser,
    selectRequestMethod,
    selectPageProps,
} from '@helpers/selectors/context'
import { postGroupDiscussion } from '@services/postGroupDiscussion'
import { formTypes } from '@constants/forms'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { withTextContent } from '@helpers/hofs/withTextContent'
import { ServerSideFormData } from '@helpers/util/form'
import { getGenericFormError } from '@helpers/util/form'
import { FormWithErrorSummary } from '@components/forms/FormWithErrorSummary'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { FormErrors, FormConfig } from '@appTypes/form'
import { useFormConfig } from '@helpers/hooks/useForm'
import { GroupPage } from '@appTypes/page'

export interface Props extends GroupPage {
    folderId: string
}

/**
 * Group create discussion template
 */
export const GroupCreateDiscussionPage: (props: Props) => JSX.Element = ({
    groupId,
    csrfToken,
    forms,
    routes,
    user,
    contentText,
    services = {
        postGroupDiscussion: postGroupDiscussion,
    },
}) => {
    const router = useRouter()

    const formConfig: FormConfig = useFormConfig(
        formTypes.CREATE_DISCUSSION,
        forms[formTypes.CREATE_DISCUSSION]
    )
    const [errors, setErrors] = useState(formConfig?.errors)

    const { secondaryHeading } = contentText ?? {}

    /**
     * Client-side submission handler
     */
    const handleSubmit = async (formData: FormData): Promise<FormErrors> => {
        try {
            await services.postGroupDiscussion({
                groupId,
                user,
                body: formData as any,
            })

            router.push(routes.groupForumRoot)

            return Promise.resolve({})
        } catch (error) {
            const errors: FormErrors =
                getServiceErrorDataValidationErrors(error) ||
                getGenericFormError(error)

            setErrors(errors)

            return Promise.resolve(errors)
        }
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
                                    submitButton: 'Create Discussion',
                                    cancelButton: 'Discard Discussion',
                                },
                            }}
                            submitAction={handleSubmit}
                            cancelHref={routes.groupForumRoot}
                            bodyClassName="u-mb-14 u-p-4 tablet:u-px-14 tablet:u-pt-12 u-pb-8 u-bg-theme-1"
                        >
                            <h2 className="nhsuk-heading-l">
                                {secondaryHeading}
                            </h2>
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
            routeId: 'fcf3d540-9a55-418c-b317-a14146ae075f',
        },
        [withUser, withRoutes, withGroup, withTextContent],
        async (context: GetServerSidePropsContext) => {
            /**
             * Get context data
             */
            const props: Partial<Props> = selectPageProps(context)
            const user: User = selectUser(context)
            const groupId: string = selectParam(context, routeParams.GROUPID)
            const csrfToken: string = selectCsrfToken(context)
            const formData: ServerSideFormData = selectFormData(context)
            const requestMethod: requestMethods = selectRequestMethod(context)

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
                !props.actions?.includes(actionConstants.GROUPS_DISCUSSIONS_ADD)
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
                        props.forms[formTypes.CREATE_DISCUSSION].errors =
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
export default GroupCreateDiscussionPage
