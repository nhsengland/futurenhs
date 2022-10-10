import { GetServerSideProps } from 'next'
import { getSession } from 'next-auth/react'
import { useRef, useState } from 'react'
import { useRouter } from 'next/router'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { withTextContent } from '@helpers/hofs/withTextContent'
import { withRoutes } from '@helpers/hofs/withRoutes'
import {
    selectCsrfToken,
    selectFormData,
    selectMultiPartFormData,
    selectPageProps,
    selectRequestMethod,
} from '@helpers/selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { formTypes } from '@constants/forms'
import { FormConfig, FormOptions } from '@appTypes/form'
import { requestMethods } from '@constants/fetch'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { FormErrors } from '@appTypes/form'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { selectFormErrors } from '@helpers/selectors/forms'
import { postRegisterSiteUser } from '@services/postRegisterSiteUser'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { Form } from '@components/forms/Form'
import { ErrorSummary } from '@components/generic/ErrorSummary'
import { getGenericFormError } from '@helpers/util/form'
import { useFormConfig } from '@helpers/hooks/useForm'
import { PageBody } from '@components/layouts/PageBody'
import { RichText } from '@components/generic/RichText'
import { GenericPageTextContent } from '@appTypes/content'
import { Page } from '@appTypes/page'
import { routes } from '@constants/routes'

declare interface ContentText extends GenericPageTextContent {
    firstNameLabel: string
    lastNameLabel: string
    pronounsLabel: string
    emailLabel: string
    editHeading?: string
    editButtonLabel?: string
    editRoleHeading?: string
    signOut: string
}

export interface Props extends Page {
    siteUser: any
    contentText: ContentText
    subjectId: string
    emailAddress: string
    issuer: string
}

const AuthRegisterPage: (props: Props) => JSX.Element = ({
    contentText,
    forms,
    csrfToken,
    routes,
    etag,
    subjectId,
    emailAddress,
    issuer,
}) => {
    const router = useRouter()
    const errorSummaryRef: any = useRef()

    //const { authSignOut } = routes;
    const profileFormConfig: FormConfig = useFormConfig(
        formTypes.REGISTER_SITE_USER,
        forms[formTypes.REGISTER_SITE_USER]
    )

    const [errors, setErrors] = useState(
        Object.assign({}, selectFormErrors(forms, formTypes.REGISTER_SITE_USER))
    )

    const { mainHeading, bodyHtml } = contentText ?? {}

    /**
     * Handle client-side validation failure in forms
     */
    const handleValidationFailure = (errors: FormErrors): void => {
        setErrors(errors)
        errorSummaryRef?.current?.focus?.()
    }

    /**
     * Handle client-side update submission for profile details
     */
    const handleProfileSubmit = async (
        formData: FormData
    ): Promise<FormErrors> => {
        return new Promise((resolve) => {
            const etagToUse: string =
                typeof etag === 'object' ? etag.profileEtag : etag

            const headers = getStandardServiceHeaders({
                csrfToken,
                etag: etagToUse,
            })

            postRegisterSiteUser({
                body: formData,
                headers,
                subjectId: subjectId,
                emailAddress: emailAddress,
                issuer: issuer,
            })
                .then(() => {
                    router.push(routes.groupsRoot)
                    setErrors({})
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

    /**
     * Render
     */
    return (
        <PageBody className="tablet:u-px-0">
            <LayoutColumnContainer justify="centre">
                <LayoutColumn tablet={8} desktop={6}>
                    <h1 className="nhsuk-heading-xl">{mainHeading}</h1>
                    <RichText bodyHtml={bodyHtml} />
                    <ErrorSummary
                        ref={errorSummaryRef}
                        errors={errors}
                        className="u-mb-10"
                    />
                    <Form
                        csrfToken={csrfToken}
                        formConfig={profileFormConfig}
                        text={{
                            submitButton: 'Save changes',
                            cancelButton: 'Discard changes',
                        }}
                        submitAction={handleProfileSubmit}
                        cancelHref={`signout`}
                        validationFailAction={handleValidationFailure}
                    />
                </LayoutColumn>
            </LayoutColumnContainer>
        </PageBody>
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
            routeId: 'a49b36cd-3423-4d5d-ae59-85a86b5a1d65',
        },
        [withRoutes, withTextContent],
        async (context: GetServerSidePropsContext) => {
            const props: Partial<Props> = selectPageProps(context)
            const csrfToken: string = selectCsrfToken(context)
            const currentValues: any = selectFormData(context)
            const submission: any = selectMultiPartFormData(context)
            const requestMethod: requestMethods = selectRequestMethod(context)
            const session = await getSession(context)
            if (!session) {
                return {
                    redirect: {
                        permanent: false,
                        destination: `${process.env.APP_URL}${routes.SIGN_IN}`,
                    },
                }
            }
            props.subjectId = session.sub as string
            props.emailAddress = session.user?.email
            props.issuer = session.iss as string

            /**
             * Hide breadcrumbs
             */
            ;(props as any).breadCrumbList = []

            props.forms = {
                [formTypes.REGISTER_SITE_USER]: {},
            }

            const profileForm: FormConfig =
                props.forms[formTypes.REGISTER_SITE_USER]

            /**
             * Get data from services
             */
            try {
                /**
                 * Handle server-side profile form post
                 */
                if (submission && requestMethod === requestMethods.POST) {
                    if (
                        currentValues.body?.['_form-id'] ===
                        formTypes.REGISTER_SITE_USER
                    ) {
                        profileForm.initialValues = currentValues.getAll()

                        const headers = {
                            ...getStandardServiceHeaders({
                                csrfToken,
                            }),
                            ...submission.getHeaders(),
                        }

                        await postRegisterSiteUser({
                            headers,
                            body: submission,
                            subjectId: session.sub as string,
                            emailAddress: session.user?.email,
                            issuer: session.iss as string,
                        })
                    }

                    return {
                        redirect: {
                            permanent: false,
                            destination: `auth/register`,
                        },
                    }
                }
            } catch (error: any) {
                const validationErrors: FormErrors =
                    getServiceErrorDataValidationErrors(error)

                if (validationErrors) {
                    profileForm.errors = validationErrors
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

export default AuthRegisterPage
